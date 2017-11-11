using System;

namespace Mingle
{
    public static class ExprOps
    {
        public static DownField DownField(this Expr expr, string key)
            => new DownField(expr, key);
    }
}