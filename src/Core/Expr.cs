using System;
using LanguageExt;

namespace Mingle
{
    public interface Expr
    {
    }

    public sealed class Doc : Record<Doc>, Expr
    {
    }

    public sealed class Var : Record<Var>, Expr
    {
        private readonly string _name;

        public Var(string name)
        {
            _name = name;
        }

        public string Name => _name;
    }

    public sealed class DownField : Record<DownField>, Expr
    {
        private readonly Expr _expr;
        private readonly string _key;

        public DownField(Expr expr, string key)
        {
            _expr = expr;
            _key = key;
        }

        public Expr Expr => _expr;

        public string Key => _key;
    }

    public sealed class Iter : Record<Iter>, Expr
    {
        private readonly Expr _expr;

        public Iter(Expr expr)
        {
            _expr = expr;
        }

        public Expr Expr => _expr;
    }

    public sealed class Next : Record<Next>, Expr
    {
        private readonly Expr _expr;

        public Next(Expr expr)
        {
            _expr = expr;
        }

        public Expr Expr => _expr;
    }
}