using System;
using System.Linq;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Mingle
{
    public /* immutable */ sealed class Replica : Record<Replica>
    {
        public readonly ReplicaId ReplicaId;
        public readonly BigInteger OpsCounter;
        public readonly Node Document;
        public readonly Map<Var, Cursor> Variables;
        public readonly Set<Id> ProcessedOps;
        public readonly Lst<Operation> GeneratedOps;
        public readonly Lst<Operation> ReceivedOps;

        private Replica(
            ReplicaId replicaId,
            BigInteger opsCounter,
            Node document,
            Map<Var, Cursor> variables,
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

        public Id CurrentId
            => new Id(OpsCounter, ReplicaId);

        public Replica ApplyCmd(Cmd cmd)
            => Replica.ApplyCmds(this, List(cmd));

        public Replica ApplyCmds(Lst<Cmd> cmds)
            => Replica.ApplyCmds(this, cmds);

        public Replica ApplyLocal(Operation operation)
            => Copy(
                document: Document.ApplyOp(operation, this),
                processedOps: ProcessedOps.TryAdd(operation.Id),
                generatedOps: GeneratedOps.Add(operation));

        // Tail recursion
        public Replica ApplyRemote()
            => match(FindApplicableRemoteOp(),
                   Some: op => Copy(
                       opsCounter: BigInteger.Max(OpsCounter, op.Id.OpsCounter),
                       document: Document.ApplyOp(op, this),
                       processedOps: ProcessedOps.Add(op.Id)).ApplyRemote(),
                   None: () => this);

        public Replica ApplyRemoteOps(Lst<Operation> ops)
            => Copy(receivedOps: ops.AddRange(ReceivedOps)).ApplyRemote();

        private Option<Operation> FindApplicableRemoteOp()
            => ReceivedOps.Find(op
                => !ProcessedOps.Contains(op.Id)
                && op.Deps.IsSubsetOf(ProcessedOps));

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
                    case Doc doc:
                        {
                            return fs.Aggregate(Cursor.Doc(), (a, f) => f(a));
                        }

                    case Var var:
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

                    case DownField df:
                        {
                            Cursor f(Cursor c)
                            {
                                switch (c.FinalKey)
                                {
                                    case HeadK hk:
                                        {
                                            return c;
                                        }
                                    default:
                                        {
                                            return c.Append(k => new MapT(k), new StrK(df.Key));
                                        }
                                }
                            }

                            return Go(df.Expr, fs.Insert(0, f));
                        }

                    case Iter it:
                        {
                            Cursor f(Cursor c)
                                => c.Append(k => new ListT(k), new HeadK());

                            return Go(it.Expr, fs.Insert(0, f));
                        }

                    case Next next:
                        {
                            Func<Cursor, Cursor> f = Document.Next;
                            return Go(next.Expr, fs.Insert(0, f));
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
                        case Let let:
                            {
                                var cur = replica.EvalExpr(let.Expr);
                                var newReplica = replica.Copy(
                                    variables: replica.Variables.AddOrUpdate(let.X, cur)
                                );
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Assign assign:
                            {
                                var newReplica = replica.MakeOp(replica.EvalExpr(assign.Expr), new AssignM(assign.Value));
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Insert ins:
                            {
                                var newReplica = replica.MakeOp(replica.EvalExpr(ins.Expr), new InsertM(ins.Value));
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Delete del:
                            {
                                var newReplica = replica.MakeOp(replica.EvalExpr(del.Expr), new DeleteM());
                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case MoveVertical mv:
                            {
                                var newReplica = replica.MakeOp(
                                    replica.EvalExpr(mv.MoveExpr),
                                    new MoveVerticalM(
                                        replica.EvalExpr(mv.MoveExpr),
                                        mv.BeforeAfter));

                                return Replica.ApplyCmds(newReplica, rest.Freeze());
                            }

                        case Sequence seq:
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

        public static Replica Empty(string replicaId)
            => Empty(new ReplicaId(replicaId));

        public static Replica Empty(ReplicaId replicaId)
            => new Replica(
                replicaId,
                opsCounter: BigInteger.Zero,
                document: Node.EmptyMap,
                variables: Map<Var, Cursor>(),
                processedOps: Set<Id>(),
                generatedOps: Lst<Operation>.Empty,
                receivedOps: Lst<Operation>.Empty);

        private Replica Copy(
            ReplicaId replicaId = null,
            BigInteger? opsCounter = null,
            Node document = null,
            Map<Var, Cursor>? variables = null,
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