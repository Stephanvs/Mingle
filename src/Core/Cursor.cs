using System;
using LanguageExt;

namespace Mingle
{
    public /* immutable */ class Cursor : Record<Cursor>
    {
        public readonly Lst<BranchTag> Keys;
        public readonly Key FinalKey;

        internal Cursor(Lst<BranchTag> keys, Key finalKey)
        {
            Keys = keys;
            FinalKey = finalKey;
        }

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
            public readonly Key FinalKey;

            public Leaf(Key finalKey)
            {
                FinalKey = finalKey;
            }
        }

        public class Branch : Record<Branch>, IView
        {
            public readonly BranchTag Head;
            public readonly Cursor Tail;

            public Branch(BranchTag head, Cursor tail)
            {
                Head = head;
                Tail = tail;
            }
        }
    }
}