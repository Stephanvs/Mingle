using System.Numerics;
using LanguageExt;

namespace Mingle
{
    public interface Val { }
    public interface LeafVal : Val { }
    public interface BranchVal : Val { }

    public sealed class Num : Record<Num>, LeafVal
    {
        public readonly BigInteger Value;

        public Num(BigInteger value)
        {
            Value = value;
        }
    }

    public sealed class Str : Record<Str>, LeafVal
    {
        public readonly string Value;

        public Str(string value)
        {
            Value = value;
        }
    }

    public sealed class True : Record<True>, LeafVal
    {
    }

    public sealed class False : Record<False>, LeafVal
    {
    }

    public sealed class Null : Record<Null>, LeafVal
    {
    }

    public sealed class EmptyList : Record<EmptyList>, BranchVal
    {
    }

    public sealed class EmptyMap : Record<EmptyMap>, BranchVal
    {
    }
}