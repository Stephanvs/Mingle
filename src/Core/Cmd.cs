using System;
using LanguageExt;

namespace Mingle
{
    public abstract class BeforeAfter { }

    public sealed class Before : BeforeAfter { }

    public sealed class After : BeforeAfter { }

    public interface Cmd : IComparable<Cmd>
    {
    }

    public sealed class Let : Record<Let>, Cmd
    {
        public readonly Var X;
        public readonly Expr Expr;

        public Let(Var x, Expr expr)
        {
            X = x;
            Expr = expr;
        }

        public int CompareTo(Cmd other)
            => RecordType<Let>.Compare(this, other as Let);
    }

    public sealed class Assign : Record<Assign>, Cmd
    {
        public readonly Expr Expr;
        public readonly Val Value;

        public Assign(Expr expr, Val value)
        {
            Expr = expr;
            Value = value;
        }

        public int CompareTo(Cmd other)
            => RecordType<Assign>.Compare(this, other as Assign);
    }

    public sealed class Insert : Record<Insert>, Cmd
    {
        public readonly Expr Expr;
        public readonly Val Value;

        public Insert(Expr expr, Val value)
        {
            Expr = expr;
            Value = value;
        }

        public int CompareTo(Cmd other)
            => RecordType<Insert>.Compare(this, other as Insert);
    }

    public sealed class Delete : Record<Delete>, Cmd
    {
        public readonly Expr Expr;

        public Delete(Expr expr)
        {
            Expr = expr;
        }

        public int CompareTo(Cmd other)
            => RecordType<Delete>.Compare(this, other as Delete);
    }

    public sealed class MoveVertical : Record<MoveVertical>, Cmd
    {
        public readonly Expr MoveExpr;
        public readonly Expr TargetExpr;
        public readonly BeforeAfter BeforeAfter;

        public MoveVertical(
            Expr moveExpr,
            Expr targetExpr,
            BeforeAfter beforeAfter)
        {
            MoveExpr = moveExpr;
            TargetExpr = targetExpr;
            BeforeAfter = beforeAfter;
        }

        public int CompareTo(Cmd other)
            => RecordType<MoveVertical>.Compare(this, other as MoveVertical);
    }

    public sealed class Sequence : Record<Sequence>, Cmd
    {
        public readonly Cmd Cmd1;
        public readonly Cmd Cmd2;

        public Sequence(Cmd cmd1, Cmd cmd2)
        {
            Cmd1 = cmd1;
            Cmd2 = cmd2;
        }

        public int CompareTo(Cmd other)
            => RecordType<Sequence>.Compare(this, other as Sequence);
    }
}