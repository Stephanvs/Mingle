using System;

namespace Mingle
{
    public abstract class BeforeAfter { }

    public class Before : BeforeAfter { }

    public class After : BeforeAfter { }

    public abstract class Cmd
    {
        public sealed class Let : Cmd
        {
            public Let(Expr.Var x, Expr expr)
            {
                X = x;
                Expr = expr;
            }

            public Expr.Var X { get; }

            public Expr Expr { get; }
        }

        public sealed class Assign : Cmd
        {
            public Assign(Expr expr, Val value)
            {
                Expr = expr;
                Value = value;
            }

            public Expr Expr { get; }

            public Val Value { get; }
        }

        public sealed class Insert : Cmd
        {
            public Insert(Expr expr, Val value)
            {
                Expr = expr;
                Value = value;
            }

            public Expr Expr { get; }

            public Val Value { get; }
        }

        public sealed class Delete : Cmd
        {
            public Delete(Expr expr)
            {
                Expr = expr;
            }

            public Expr Expr { get; }
        }

        public sealed class MoveVertical : Cmd
        {
            public MoveVertical(
                Expr moveExpr,
                Expr targetExpr,
                BeforeAfter beforeAfter)
            {
                MoveExpr = moveExpr;
                TargetExpr = targetExpr;
                BeforeAfter = beforeAfter;
            }

            public Expr MoveExpr { get; }

            public Expr TargetExpr { get; }

            public BeforeAfter BeforeAfter { get; }
        }

        public sealed class Sequence : Cmd
        {
            public Sequence(Cmd cmd1, Cmd cmd2)
            {
                Cmd1 = cmd1;
                Cmd2 = cmd2;
            }

            public Cmd Cmd1 { get; }

            public Cmd Cmd2 { get; }
        }
    }
}