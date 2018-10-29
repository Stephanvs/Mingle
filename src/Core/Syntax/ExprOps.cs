using System;

namespace Mingle
{
    public static class ExprOps
    {
        public static Cmd Assign(this Expr expr, string value)
            => new Assign(expr, new Str(value));

        public static Cmd Assign(this Expr expr, Val value)
            => new Assign(expr, value);

        public static DownField DownField(this Expr expr, string key)
            => new DownField(expr, key);

        public static Insert Insert(this Expr expr, Val value)
            => new Insert(expr, value);

        public static Insert Insert(this Expr expr, string value)
            => new Insert(expr, new Str(value));

        public static Insert Insert(this Expr expr, bool value)
            => new Insert(expr, value ? (Val)new True() : (Val)new False());

        public static Delete Delete(this Expr expr)
            => new Delete(expr);

        public static Iter Iter(this Expr expr)
            => new Iter(expr);

        public static Next Next(this Expr expr)
            => new Next(expr);
    }
}