using System;

namespace Mingle
{
    public static class CmdOps
    {
        public static Cmd Assign(this Expr expr, string value)
            => new Assign(expr, new Str(value));

        public static Cmd Assign(this Expr expr, Val value)
            => new Assign(expr, value);
    }
}