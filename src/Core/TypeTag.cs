using System;

namespace Mingle
{
    public interface TypeTag
    {
        Key Key { get; }
    }

    public abstract class BranchTag : TypeTag
    {
        protected BranchTag(Key key)
        {
            Key = key;
        }

        public Key Key { get; }
    }

    public sealed class MapT : BranchTag
    {
        public MapT(Key key) : base(key)
        {
        }
    }

    public sealed class ListT : BranchTag
    {
        public ListT(Key key) : base(key)
        {
        }
    }

    public sealed class RegT : TypeTag
    {
        public RegT(Key key)
        {
            Key = key;
        }

        public Key Key { get; }
    }
}