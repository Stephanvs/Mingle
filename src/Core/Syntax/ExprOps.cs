using System;

namespace Mingle
{
    public static class ExprOps
    {
        public static Expr DownField(this Expr expr, string key)
            => new Expr.DownField(expr, key);
    }
}