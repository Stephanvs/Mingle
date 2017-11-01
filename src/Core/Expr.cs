using System;

namespace Mingle
{
    public abstract class Expr
    {
        public class Doc : Expr
        {
        }

        public sealed class Var : Expr
        {
            public Var(string name)
            {
                Name = name;
            }

            public string Name { get; }
        }

        public sealed class DownField : Expr
        {
            public DownField(Expr expr, string key)
            {
                Expr = expr;
                Key = key;
            }

            public Expr Expr { get; }

            public string Key { get; }
        }

        public sealed class Iter : Expr
        {
            public Iter(Expr expr)
            {
                Expr = expr;
            }

            public Expr Expr { get; }
        }

        public sealed class Next : Expr
        {
            public Next(Expr expr)
            {
                Expr = expr;
            }

            public Expr Expr { get; }
        }
    }
}