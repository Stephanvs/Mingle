using System;
using LanguageExt;

namespace Mingle
{
    public /* immutable */ class Cursor
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
            => WithFinalKey(new DocK());

        public static Cursor WithFinalKey(Key finalKey)
            => new Cursor(Lst<BranchTag>.Empty, finalKey);

        public interface IView
        {
        }

        public class Leaf : Record<Leaf>, IView
        {
            private readonly Key _finalKey;

            public Leaf(Key finalKey)
            {
                _finalKey = finalKey;
            }

            public Key FinalKey => _finalKey;
        }

        public class Branch : Record<Branch>, IView
        {
            private readonly BranchTag _head;
            private readonly Cursor _tail;

            public Branch(BranchTag head, Cursor tail)
            {
                _head = head;
                _tail = tail;
            }

            public BranchTag Head => _head;

            public Cursor Tail => _tail;
        }
    }
}