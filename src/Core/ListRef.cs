using System;

namespace Mingle
{
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
    }

    public abstract class ListRef
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
    }

    public sealed class IdR : KeyRef
    {
        public IdR(Id id)
        {
            Id = id;
        }

        public Id Id { get; }
    }

    public sealed class HeadR : KeyRef { }

    public sealed class TailR : ListRef { }
}