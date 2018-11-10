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
        public readonly string Name;

        public Var(string name)
        {
            Name = name;
        }
    }

    public sealed class DownField : Record<DownField>, Expr
    {
        public readonly Expr Expr;
        public readonly string Key;

        public DownField(Expr expr, string key)
        {
            Expr = expr;
            Key = key;
        }
    }

    public sealed class Iter : Record<Iter>, Expr
    {
        public readonly Expr Expr;

        public Iter(Expr expr)
        {
            Expr = expr;
        }
    }

    public sealed class Next : Record<Next>, Expr
    {
        public readonly Expr Expr;

        public Next(Expr expr)
        {
            Expr = expr;
        }
    }
}