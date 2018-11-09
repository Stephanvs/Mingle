using System;
using System.Numerics;
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
        public readonly Id Id;

        public IdK(BigInteger opsCounter, string replicaId)
            : this(new Id(opsCounter, replicaId)) {}

        public IdK(Id id)
        {
            Id = id;
        }

        public int CompareTo(object obj)
        {
            return obj is IdK id
                ? id.Id.CompareTo(this.Id)
                : 0;
        }
    }

    public sealed class StrK : Record<StrK>, Key
    {
        public readonly string Str;

        public StrK(string str)
        {
            Str = str;
        }

        public int CompareTo(object obj)
        {
            return obj is StrK k
                ? k.Str.CompareTo(this.Str)
                : 0;
        }
    }
}