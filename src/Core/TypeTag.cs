using System;
using LanguageExt;

namespace Mingle
{
    public interface TypeTag : IComparable
    {
        Key Key { get; }
    }

    public interface BranchTag : TypeTag
    {
    }

    public sealed class MapT : Record<MapT>, BranchTag
    {
        public MapT(Key key)
        {
            Key = key;
        }

        public Key Key { get; }

        public int CompareTo(object obj)
        {
            return (obj is MapT o)
                ? o.Key.CompareTo(this.Key)
                : 0;
        }
    }

    public sealed class ListT : Record<ListT>, BranchTag
    {
        public ListT(Key key)
        {
            Key = key;
        }

        public Key Key { get; }

        public int CompareTo(object obj)
        {
            return (obj is ListT o)
                ? o.Key.CompareTo(this.Key)
                : 0;
        }
    }

    public sealed class RegT : Record<RegT>, TypeTag
    {
        public RegT(Key key)
        {
            Key = key;
        }

        public Key Key { get; }

        public int CompareTo(object obj)
        {
            return (obj is RegT o)
                ? o.Key.CompareTo(this.Key)
                : 0;
        }
    }
}