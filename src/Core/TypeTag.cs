using System;
using LanguageExt;

namespace Mingle
{
    public interface TypeTag
    {
        Key Key { get; }
    }

    public interface BranchTag : TypeTag
    {
    }

    public sealed class MapT : Record<MapT>, BranchTag, IComparable
    {
        public readonly Key _key;

        public MapT(Key key)
        {
            _key = key;
        }

        public Key Key => _key;

        // public int CompareTo(object obj)
        // {
        //     return (obj is MapT o)
        //         ? o.Key.CompareTo(this.Key)
        //         : 0;
        // }
        public int CompareTo(object obj)
            => RecordType<MapT>.Compare(this, obj as MapT);
    }

    public sealed class ListT : Record<ListT>, BranchTag, IComparable
    {
        public readonly Key _key;

        public ListT(Key key)
        {
            _key = key;
        }

        public Key Key => _key;

        // public int CompareTo(object obj)
        // {
        //     return (obj is ListT o)
        //         ? o.Key.CompareTo(this.Key)
        //         : 0;
        // }
        public int CompareTo(object obj)
            => RecordType<ListT>.Compare(this, obj as ListT);
    }

    public sealed class RegT : Record<RegT>, TypeTag, IComparable
    {
        public readonly Key _key;

        public RegT(Key key)
        {
            _key = key;
        }

        public Key Key => _key;

        // public int CompareTo(object obj)
        // {
        //     return (obj is RegT o)
        //         ? o.Key.CompareTo(this.Key)
        //         : 0;
        // }
        public int CompareTo(object obj)
            => RecordType<RegT>.Compare(this, obj as RegT);
    }
}