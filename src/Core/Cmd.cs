using System;
using LanguageExt;

namespace Mingle
{
    public abstract class BeforeAfter { }

    public sealed class Before : BeforeAfter { }

    public sealed class After : BeforeAfter { }

    public interface Cmd
    {
    }

    public sealed class Let : Record<Let>, Cmd
    {
        private readonly Var _x;
        private readonly Expr _expr;

        public Let(Var x, Expr expr)
        {
            _x = x;
            _expr = expr;
        }

        public Var X => _x;

        public Expr Expr => _expr;
    }

    public sealed class Assign : Record<Assign>, Cmd
    {
        private readonly Expr _expr;
        private readonly Val _value;

        public Assign(Expr expr, Val value)
        {
            _expr = expr;
            _value = value;
        }

        public Expr Expr => _expr;

        public Val Value => _value;
    }

    public sealed class Insert : Record<Insert>, Cmd
    {
        private readonly Expr _expr;
        private readonly Val _value;

        public Insert(Expr expr, Val value)
        {
            _expr = expr;
            _value = value;
        }

        public Expr Expr => _expr;

        public Val Value => _value;
    }

    public sealed class Delete : Record<Delete>, Cmd
    {
        private readonly Expr _expr;

        public Delete(Expr expr)
        {
            _expr = expr;
        }

        public Expr Expr => _expr;
    }

    public sealed class MoveVertical : Record<MoveVertical>, Cmd
    {
        private readonly Expr _moveExpr;
        private readonly Expr _targetExpr;
        private readonly BeforeAfter _beforeAfter;

        public MoveVertical(
            Expr moveExpr,
            Expr targetExpr,
            BeforeAfter beforeAfter)
        {
            _moveExpr = moveExpr;
            _targetExpr = targetExpr;
            _beforeAfter = beforeAfter;
        }

        public Expr MoveExpr => _moveExpr;

        public Expr TargetExpr => _targetExpr;

        public BeforeAfter BeforeAfter => _beforeAfter;
    }

    public sealed class Sequence : Record<Sequence>, Cmd
    {
        private readonly Cmd _cmd1;
        private readonly Cmd _cmd2;

        public Sequence(Cmd cmd1, Cmd cmd2)
        {
            _cmd1 = cmd1;
            _cmd2 = cmd2;
        }

        public Cmd Cmd1 => _cmd1;

        public Cmd Cmd2 => _cmd2;
    }
}