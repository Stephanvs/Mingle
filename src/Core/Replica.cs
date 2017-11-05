using System;
using System.Linq;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Mingle
{
    public /* immutable */ sealed class Replica
    {
        private Replica(
            ReplicaId replicaId,
            bigint opsCounter,
            Node document,
            Map<Expr.Var, Cursor> variables,
            Set<Id> processedOps,
            Lst<Operation> generatedOps,
            Lst<Operation> receivedOps)
        {
            OpsCounter = opsCounter;
            Document = document;
            ReplicaId = replicaId;
            Variables = variables;
            ProcessedOps = processedOps;
            GeneratedOps = generatedOps;
            ReceivedOps = receivedOps;
        }

        public ReplicaId ReplicaId { get; }

        public bigint OpsCounter { get; }

        public Node Document { get; }

        public Map<Expr.Var, Cursor> Variables { get; }

        public Set<Id> ProcessedOps { get; }

        public Lst<Operation> GeneratedOps { get; }

        public Lst<Operation> ReceivedOps { get; }

        public Id CurrentId
            => new Id(OpsCounter, ReplicaId);

        public Replica ApplyCmd(Cmd cmd)
            => Replica.ApplyCmds(this, List(cmd));

        public Replica ApplyCmds(Lst<Cmd> cmds)
            => Replica.ApplyCmds(this, cmds);

        public Replica ApplyLocal(Operation operation)
            => Copy(
                document: Document.ApplyOp(operation, this),
                processedOps: ProcessedOps.Add(operation.Id),
                generatedOps: GeneratedOps.Add(operation));

        // Tail recursion
        public Replica ApplyRemote()
            => match(FindApplicableRemoteOp(),
                   Some: op => Copy(
                       opsCounter: bigint.Max(OpsCounter, op.Id.OpsCounter),
                       document: Document.ApplyOp(op, this),
                       processedOps: ProcessedOps.Add(op.Id)).ApplyRemote(),
                   None: () => this);

        public Replica ApplyRemoteOps(Lst<Operation> ops)
            => Copy(receivedOps: ops.AddRange(ReceivedOps)).ApplyRemote();

        private Option<Operation> FindApplicableRemoteOp()
        {
            return ReceivedOps.Find(op
                => !ProcessedOps.Contains(op.Id)
                && op.Deps.IsSubsetOf(ProcessedOps));
        }

        public Replica IncrementCounter()
            => Copy(opsCounter: OpsCounter + 1);

        private Set<string> Keys(Expr expr)
            => Document.Keys(EvalExpr(expr));

        private Replica MakeOp(Cursor cursor, Mutation mutation)
        {
            var replica = IncrementCounter();
            var op = new Operation(replica.CurrentId, replica.ProcessedOps, cursor, mutation);

            return replica.ApplyLocal(op);
        }

        private Lst<LeafVal> Values(Expr expr)
            => Document.Values(EvalExpr(expr));

        public Cursor EvalExpr(Expr expr)
        {
            Cursor Go(Expr ex, Lst<Func<Cursor, Cursor>> fs)
            {
                switch (ex)
                {
                    case Expr.Doc doc:
                        {
                            return fs.Aggregate(Cursor.Doc(), (a, f) => f(a));
                        }

                    case Expr.Var var:
                        {
                            // return Variables.Match(var)
                            //     .Some(cur => cur)
                            //     .None(Cursor.Doc());
                            // Variables[var]
                            // NOTE: This is not correct, I think I should iterate over each element in the `Variables` map, and foreach element lookup the 
                            // return match (Variables,
                            //     Some: cur => cur,
                            //     None: Cursor.Doc());
                            throw new InvalidOperationException();
                        }

                    case Expr.DownField df:
                        {
                            Cursor Func(Cursor c)
                            {
                                switch (c.FinalKey)
                                {
                                    case Key.HeadK hk:
                                        {
                                            return c;
                                        }
                                    default:
                                        {
                                            return c.Append(k => new MapT(k), new Key.StrK(df.Key));
                                        }
                                }
                            }

                            return Go(df.Expr, fs.Add(Func));
                        }

                    case Expr.Iter it:
                        {
                            Cursor Func(Cursor c)
                                => c.Append(k => new ListT(k), new Key.HeadK());

                            return Go(it.Expr, fs.Add(Func));
                        }

                    case Expr.Next next:
                        {
                            Func<Cursor, Cursor> func = Document.Next;
                            return Go(next.Expr, fs.Add(func));
                        }

                    default:
                        throw new InvalidOperationException();
                }
            }

            return Go(expr, List<Func<Cursor, Cursor>>());
        }

        public static Replica ApplyCmds(Replica replica, Lst<Cmd> cmds)
            => match(cmds,
                () => replica,
                (cmd, rest) =>
                {
                    switch (cmd)
                    {
                        case Cmd.Let let:
                            {
                                var cur = replica.EvalExpr(let.Expr);
                                var newReplica = replica.Copy(
                                    variables: replica.Variables.AddOrUpdate(let.X, cur)
                                );
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Cmd.Assign assign:
                            {
                                var newReplica = replica.MakeOp(replica.EvalExpr(assign.Expr), new AssignM(assign.Value));
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Cmd.Insert ins:
                            {
                                var newReplica = replica.MakeOp(replica.EvalExpr(ins.Expr), new InsertM(ins.Value));
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Cmd.Delete del:
                            {
                                var newReplica = replica.MakeOp(replica.EvalExpr(del.Expr), new DeleteM());
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Cmd.MoveVertical mv:
                            {
                                var newReplica = replica.MakeOp(
                                    replica.EvalExpr(mv.MoveExpr),
                                    new MoveVerticalM(
                                        replica.EvalExpr(mv.MoveExpr),
                                        mv.BeforeAfter));

                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Cmd.Sequence seq:
                            {
                                return Replica.ApplyCmds(
                                    replica,
                                    List(seq.Cmd1, seq.Cmd2)
                                        .Append(rest)
                                        .Freeze());
                            }
                    }
                    return replica;
                });

        public static Replica Empty(ReplicaId replicaId)
            => new Replica(
                replicaId,
                opsCounter: bigint.Zero,
                document: Node.EmptyMap,
                variables: Map<Expr.Var, Cursor>(),
                processedOps: Set<Id>(),
                generatedOps: Lst<Operation>.Empty,
                receivedOps: Lst<Operation>.Empty);

        private Replica Copy(
            ReplicaId replicaId = null,
            bigint? opsCounter = null,
            Node document = null,
            Map<Expr.Var, Cursor>? variables = null,
            Set<Id>? processedOps = null,
            Lst<Operation>? generatedOps = null,
            Lst<Operation>? receivedOps = null)
        {
            return new Replica(
                replicaId ?? ReplicaId,
                opsCounter ?? OpsCounter,
                document ?? Document,
                variables ?? Variables,
                processedOps ?? ProcessedOps,
                generatedOps ?? GeneratedOps,
                receivedOps ?? ReceivedOps);
        }
    }
}