using System;
using System.Collections.Generic;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Mingle
{
    public /* immutable */ sealed class Cursor : Record<Cursor>
    {
        public readonly Lst<BranchTag> Keys;
        public readonly Key FinalKey;

        internal Cursor(Lst<BranchTag> keys, Key finalKey)
        {
            Keys = keys;
            FinalKey = finalKey;
        }

        public Cursor Append(Func<Key, BranchTag> tag, Key newFinalKey)
            => new Cursor(Keys.Add(tag(FinalKey)), newFinalKey);

        public Cursor.IView View()
            => match<BranchTag, Cursor.IView>(Keys,
                () => new Leaf(FinalKey),
                (k1, kn) => new Branch(k1, new Cursor(kn.Freeze(), FinalKey)));

        public static Cursor Doc()
            => WithFinalKey(new DocK());

        public static Cursor WithFinalKey(Key finalKey)
            => new Cursor(Lst<BranchTag>.Empty, finalKey);

        internal Cursor Copy(Lst<BranchTag>? keys = null, Key finalKey = null)
            => new Cursor(keys: keys ?? Keys, finalKey: finalKey ?? FinalKey);

        public interface IView
        {
        }

        public sealed class Leaf : Record<Leaf>, IView
        {
            public readonly Key FinalKey;

            public Leaf(Key finalKey)
            {
                FinalKey = finalKey;
            }
        }

        public sealed class Branch : Record<Branch>, IView
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