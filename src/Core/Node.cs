using System;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Mingle
{
    public abstract class Node
    {
        public static Node EmptyMap
            => new MapNode(
                Map<TypeTag, Node>(),
                Map<Key, Set<Id>>());

        public Cursor Next(Cursor cursor)
        {
            switch (cursor.View())
            {
                case Cursor.Leaf l:
                    {
                        // TODO: Replace variable decl with `var`
                        ListRef k1Ref = GetNextRef(ListRef.FromKey(l.FinalKey));

                        switch (k1Ref)
                        {
                            case TailR _: return cursor;
                            case KeyRef keyRef:
                                {
                                    var k1 = keyRef.ToKey();
                                    var cur1 = Cursor.WithFinalKey(k1);
                                    // NEXT2
                                    if (!GetPres(k1).IsEmpty) { return cur1; }
                                    // NEXT3
                                    else { return Next(cur1); }
                                }
                            default: { throw new InvalidOperationException(); }
                        }
                    }

                // NEXT4
                case Cursor.Branch b:
                    {
                        // FindChild(b.Head);
                        // var cur2 = ;
                        // return cursor.Copy(finalKey: cur2.FinalKey);
                        throw new InvalidOperationException();
                    }

                default:
                    {
                        throw new InvalidOperationException(
                            $"Unexpected Cursor.View() type found '{cursor.View().GetType().Name}'");
                    }
            }
        }

        public Set<string> Keys(Cursor cursor)
        {
            throw new NotImplementedException();
        }

        public Lst<LeafVal> Values(Cursor cursor)
        {
            throw new NotImplementedException();
        }

        public Node ApplyOp(Operation operation, Replica replica)
        {
            var view = operation.Cursor.View();
            switch (view)
            {
                case Cursor.Leaf l:
                    {
                        break;
                    }
                case Cursor.Branch b:
                    {
                        break;
                    }
            }

            return this;
        }

        private Set<Id> GetPres(Key key)
        {
            switch (this)
            {
                case BranchNode n: return n.PresSets.ContainsKey(key) ? n.PresSets[key] : Set<Id>();
                default: return Set<Id>();
            }
        }

        private ListRef GetNextRef(ListRef @ref)
        {
            switch (this)
            {
                case ListNode ln: { return ln.Order.ContainsKey(@ref) ? ln.Order[@ref] : new TailR(); }
                default: return new TailR();
            }
        }
    }

    public abstract class BranchNode : Node
    {
        public BranchNode(
            Map<TypeTag, Node> children,
            Map<Key, Set<Id>> presSets)
        {
            Children = children;
            PresSets = presSets;
        }

        public virtual Map<TypeTag, Node> Children { get; }

        public virtual Map<Key, Set<Id>> PresSets { get; }

        public abstract BranchNode WithChildren(Map<TypeTag, Node> children);

        public abstract BranchNode WithPresSets(Map<Key, Set<Id>> presSets);
    }

    public class MapNode : BranchNode
    {
        public MapNode(
            Map<TypeTag, Node> children,
            Map<Key, Set<Id>> presSets)
            : base(children, presSets)
        {
        }

        public override BranchNode WithChildren(Map<TypeTag, Node> children)
        {
            throw new NotImplementedException();
        }

        public override BranchNode WithPresSets(Map<Key, Set<Id>> presSets)
        {
            throw new NotImplementedException();
        }
    }

    public class ListNode : BranchNode
    {
        public ListNode(
            Map<TypeTag, Node> children,
            Map<Key, Set<Id>> presSets,
            Map<ListRef, ListRef> order,
            Map<bigint, Map<ListRef, ListRef>> orderArchive)
            : base(children, presSets)
        {
            Order = order;
            OrderArchive = orderArchive;
        }

        public Map<ListRef, ListRef> Order { get; }

        public Map<bigint, Map<ListRef, ListRef>> OrderArchive { get; }

        public override BranchNode WithChildren(Map<TypeTag, Node> children)
        {
            throw new NotImplementedException();
        }

        public override BranchNode WithPresSets(Map<Key, Set<Id>> presSets)
        {
            throw new NotImplementedException();
        }

        /** The tests cannot converge, since the orderArchive of two replicas is
        * always different. Therefore don't compare the orderArchive.
        */
        public override bool Equals(object obj)
        {
            if (obj is ListNode ln)
            {
                return ln.Children == Children
                    && ln.PresSets == PresSets
                    && ln.Order == Order;
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }

    public class RegNode : Node
    {
        public RegNode(Map<Id, LeafVal> regValues)
        {
            RegValues = regValues;
        }

        public Map<Id, LeafVal> RegValues { get; }

        public Lst<LeafVal> Values()
            => new Lst<LeafVal>(RegValues.Values);
    }
}