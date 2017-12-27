using System;
using LanguageExt;

namespace Mingle
{
    public interface Key : IComparable
    {
    }

    public class DocK : Record<DocK>, Key
    {
        public int CompareTo(object obj) => 0;
    }

    public class HeadK : Record<HeadK>, Key
    {
        public int CompareTo(object obj) => 0;
    }

    public sealed class IdK : Record<IdK>, Key
    {
        private readonly Id _id;

        public IdK(Id id)
        {
            _id = id;
        }

        public Id Id => _id;

        public int CompareTo(object obj)
        {
            return obj is IdK id
                ? id.Id.CompareTo(this.Id)
                : 0;
        }
    }

    public sealed class StrK : Record<StrK>, Key
    {
        private readonly string _str;

        public StrK(string str)
        {
            _str = str;
        }

        public string Str => _str;

        public int CompareTo(object obj)
        {
            return obj is StrK k
                ? k.Str.CompareTo(this.Str)
                : 0;
        }
    }
}