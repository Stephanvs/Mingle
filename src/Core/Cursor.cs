using System;
using LanguageExt;

namespace Mingle
{
    public /*immutable */ class Cursor
    {
        private Cursor(Lst<BranchTag> keys, Key finalKey)
        {
            Keys = keys;
            FinalKey = finalKey;
        }

        public Lst<BranchTag> Keys { get; }

        public Key FinalKey { get; }

        public Cursor Append(Func<Key, BranchTag> tag, Key newFinalKey)
        {
            var branchTag = tag(FinalKey);
            return new Cursor(Keys.Add(branchTag), newFinalKey);
        }

        public Cursor.IView View()
        {
            return new Leaf(FinalKey);
        }

        public static Cursor Doc()
            => WithFinalKey(new Key.DocK());

        public static Cursor WithFinalKey(Key finalKey)
            => new Cursor(Lst<BranchTag>.Empty, finalKey);

        public interface IView
        {
        }

        public class Leaf : IView
        {
            public Leaf(Key finalKey)
            {
                FinalKey = finalKey;
            }

            public Key FinalKey { get; }
        }

        public class Branch : IView
        {
            public Branch(BranchTag head, Cursor tail)
            {
                Head = head;
                Tail = tail;
            }

            public BranchTag Head { get; }

            public Cursor Tail { get; }
        }
    }
}