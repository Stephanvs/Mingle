using System;
using LanguageExt;

namespace Mingle
{
    public abstract class ListRef : Record<ListRef>, IComparable
    {
        public static ListRef FromKey(Key key)
        {
            switch (key)
            {
                case IdK k: return new IdR(k.Id);
                case HeadK _: return new HeadR();
                default: return new TailR();
            }
        }

        public abstract int CompareTo(object other);
    }

    public abstract class KeyRef : ListRef
    {
        public Key ToKey()
        {
            switch (this)
            {
                case IdR r: return new IdK(r.Id);
                case HeadR _: return new HeadK();
            }

            throw new InvalidOperationException(
                $"Cannot convert {this.GetType().Name} using method '{nameof(ToKey)}' to {nameof(Key)}. " +
                $"Are you illegaly subclassing {nameof(KeyRef)}?");
        }

        public override int CompareTo(object other)
            => RecordType<KeyRef>.Compare(this, other as KeyRef);
    }

    public sealed class IdR : KeyRef
    {
        public readonly Id Id;

        public IdR(Id id)
        {
            Id = id;
        }

        public override int CompareTo(object other)
            => RecordType<IdR>.Compare(this, other as IdR);
    }

    public sealed class HeadR : KeyRef
    {
        public override int CompareTo(object other)
            => RecordType<HeadR>.Compare(this, other as HeadR);
    }

    public sealed class TailR : ListRef
    {
        public override int CompareTo(object other)
            => RecordType<TailR>.Compare(this, other as TailR);
    }
}