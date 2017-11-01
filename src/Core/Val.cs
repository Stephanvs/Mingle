using System;
using System.Numerics;
using LanguageExt;

namespace Mingle
{
    public interface Val { }
    public interface LeafVal : Val { }
    public interface BranchVal : Val { }

    public sealed class Num : LeafVal
    {
        public Num(bigint value)
        {
            Value = value;
        }

        public bigint Value { get; }
    }

    public sealed class Str : LeafVal
    {
        public Str(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}