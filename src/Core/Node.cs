using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Mingle
{
    public abstract class Node : Record<Node>, IComparable
    {
        public static Node EmptyMap
            => new MapNode(
                LanguageExt.Map<TypeTag, Node>.Empty,
                LanguageExt.Map<Key, Set<Id>>.Empty);

        public static Node EmptyList
            => new ListNode(
                LanguageExt.Map<TypeTag, Node>.Empty,
                LanguageExt.Map<Key, Set<Id>>.Empty,
                LanguageExt.Map<ListRef, ListRef>.Empty,
                LanguageExt.Map<BigInteger, Map<ListRef, ListRef>>.Empty);

        public static Node EmptyReg
            => new RegNode(LanguageExt.Map<Id, LeafVal>.Empty);

        public Cursor Next(Cursor cursor)
        {
            switch (cursor.View())
            {
                case Cursor.Leaf l:
                    {
                        var k1Ref = GetNextRef(ListRef.FromKey(l.FinalKey));

                        switch (k1Ref)
                        {
                            case TailR _: return cursor;
                            case KeyRef keyRef:
                                {
                                    var k1 = keyRef.ToKey();
                                    var cur1 = Cursor.WithFinalKey(k1);
                                    // NEXT2
                                    if (GetPres(k1).Any()) { return cur1; }
                                    // NEXT3
                                    else { return Next(cur1); }
                                }
                            default: { throw new InvalidOperationException(); }
                        }
                    }

                // NEXT4
                case Cursor.Branch b:
                    {
                        return FindChild(b.Head).Fold(cursor, (s, child) =>
                        {
                            var cur2 = child.Next(s);
                            return cursor.Copy(finalKey: cur2.FinalKey);
                        });
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

        /** If this node is a list: Save the order of item in the list.
        * Don't overwrite if it already exists. Why? Assumed it would be overwritten
        * and user1, user2 and user3 do an op concurrently. When user2's op arrives
        * at user1, the order is reset and both ops are redone. Each time an op is
        * applied, the order is saved. Therefore when the user2's op is applied, the
        * order is saved. That's the order after user1's op was applied. When now
        * user3's op arrives and the order is reset, the order we reset to should be
        * the order before all three ops were applied. But it's not, since user2's op
        * has overwritten the order. Therefore don't overwrite. */
        private Node SaveOrder(Operation operation)
        {
            switch (this)
            {
                case ListNode ln:
                {
                    if (ln.OrderArchive.Exists((k, v) => k == operation.Id.OpsCounter && v.IsEmpty))
                    {
                        return ln.Copy(orderArchive: ln.OrderArchive.Add(operation.Id.OpsCounter, ln.Order));
                    }
                    return this;
                }
                default: return this;
            }
        }

        public Node ApplyOp(Operation op, Replica replica)
        {
            var view = op.Cursor.View();
            switch (view)
            {
                case Cursor.Leaf l:
                    {
                        switch (this)
                        {
                            case ListNode ln:
                                {
                                    IEnumerable<Operation> ConcurrentOpsSince(BigInteger count)
                                    {
                                        var allOps = replica.GeneratedOps.Append(replica.ReceivedOps);

                                        foreach (var o in allOps)
                                        {
                                            if ((replica.ProcessedOps.Contains(o.Id) &&
                                                (o.Mutation is InsertM || o.Mutation is DeleteM || o.Mutation is MoveVerticalM) &&
                                                o.Id == op.Id) &&
                                                o.Id.OpsCounter >= count &&
                                                this.FindChild(new RegT(o.Cursor.FinalKey)).IsSome)
                                            {
                                                yield return o;
                                            }
                                        }
                                    }

                                    var concurrentOps = ConcurrentOpsSince(op.Id.OpsCounter);

                                    if (concurrentOps.Length() > 1
                                        && concurrentOps.Select(x => x.Mutation).OfType<MoveVerticalM>().Length() >= 1)
                                    {
                                        //  Before applying an operation we save the order in orderArchive.
                                        // It's a Map whose key is the lamport timestamp counter value.
                                        // To improve performance and save disk space, we don't save the
                                        // order before assign operations, since they don't change the order.
                                        // Now there might be this situation: Alice did an assign and then a
                                        // move op, while Bob did a move op. Now Bobs op comes in and
                                        // Alice resets her order to the order with counter value like the
                                        // incoming op. However, locally exists no such saved order, since
                                        // she has done an assign op at that count. Therefore she resets
                                        // to the next higher saved order.
                                        // This fix is implemented by getting all orders whose counter is
                                        // greater equals than the counter of the incoming op and then
                                        // choosing the earliest order of those:
                                        var newerOrders = ln.OrderArchive.Filter((k, v) => k >= op.Id.OpsCounter);

                                        // restore the order
                                        // TODO:
                                        // val ctx1 =
                                        //     if (newerOrders.nonEmpty) ln.copy(order = newerOrders.minBy {
                                        //         case (c, _) => c
                                        //     }._2)
                                        //     else this
                                        throw new NotImplementedException();

                                        // Node ctx1 = this;

                                        // return ctx1.ApplyMany(concurrentOps.OrderBy(x => x.Id));
                                    }
                                    else
                                    {
                                        // The op was done without me doing an op concurrently, so there is
                                        // no need to restore anything. Just apply the op.
                                        return ApplyAtLeaf(op, replica);
                                    }
                                }
                            default: return ApplyAtLeaf(op, replica);
                        }
                        throw new InvalidOperationException("Invalid Cursor type");
                    }
                case Cursor.Branch bn:
                    {
                        var child0 = GetChild(bn.Head);
                        var child1 = child0.ApplyOp(op.Copy(cursor: bn.Tail), replica);
                        var ctx1 = AddId(bn.Head, op.Id, op.Mutation);
                        return ctx1.AddNode(bn.Head, child1);
                    }
            }

            return this;
        }

        private Node ApplyAtLeaf(Operation op, Replica replica)
        {
            var k = op.Cursor.FinalKey;

            switch (op.Mutation)
            {
                case AssignM ass:
                    {
                        if (ass.Value is BranchVal)
                        {
                            BranchTag tag;

                            if (ass.Value.Equals(EmptyMap))
                            {
                                tag = new MapT(k);
                            }
                            else
                            {
                                tag = new ListT(k);
                            }

                            var (ctx1, _) = ClearElem(op.Deps, k);
                            var ctx2 = ctx1.AddId(tag, op.Id, op.Mutation);
                            var child = ctx2.GetChild(tag);

                            return ctx2.AddNode(tag, child);
                        }
                        else if (ass.Value is LeafVal leafVal)
                        {
                            var tag = new RegT(k);
                            var (ctx1, _) = Clear(op.Deps, tag);
                            var ctx2 = ctx1.AddId(tag, op.Id, op.Mutation);
                            var child = ctx2.GetChild(tag);

                            return ctx2.AddNode(tag, child.AddRegValue(op.Id, leafVal));
                        }

                        throw new InvalidOperationException(
                            $"Assingment of value type {ass.Value.GetType().Name} is invalid");
                    }
                case DeleteM del:
                    {
                        throw new NotImplementedException();
                    }
                case InsertM ins:
                    {
                        var prevRef = ListRef.FromKey(k);
                        var nextRef = GetNextRef(prevRef);
                        switch (nextRef)
                        {
                            // INSERT2
                            // INSERT 2 handles the case of multiple replicas concurrently
                            // inserting list elements at the same position, and uses the
                            // ordering relation < on Lamport timestamps to consistently
                            // determine the insertion point.
                            case IdR nextId:
                            {
                                // Normally, the nextId is lower, since it was already inserted and
                                // newer ops have a higher id. NextId may be only higher, if two
                                // insert operations are concurrent the one the other one - with the
                                // higher id - was already inserted. When that's the case, insert
                                // the new op after the concurrent one with higher id. This way, when
                                // inserted at the same place, the op whose user id is higher,
                                // comes always fist.
                                if (op.Id > nextId.Id)
                                    return ApplyAtLeaf(op.Copy(cursor: Cursor.WithFinalKey(new IdK(nextId.Id))), replica);
                                else goto default;
                            }
                            // INSERT1
                            // INSERT1 performs the insertion by manipulating the linked
                            // list structure.
                            default:
                            {
                                var idRef = new IdR(op.Id);
                                // the ID of the inserted node will be the ID of the operation
                                var ctx1 = ApplyAtLeaf(
                                    op.Copy(cursor: Cursor.WithFinalKey(new IdK(op.Id)),
                                            mutation: new AssignM(ins.Value)),
                                    replica);
                                var ctx2 = ctx1.SaveOrder(op);
                                return ctx2
                                    .SetNextRef(prevRef, idRef)
                                    .SetNextRef(idRef, nextRef);
                            }
                        }
                    }
                case MoveVerticalM mv:
                    {
                        throw new NotImplementedException();
                    }
                default: throw new InvalidOperationException("Invalid Mutation occurred.");
            }
        }

        private Node AddNode(TypeTag tag, Node node)
        {
            switch (this)
            {
                case BranchNode n: return n.WithChildren(n.Children.AddOrUpdate(tag, node));
                default: return this;
            }
        }

        private Node AddRegValue(Id id, LeafVal value)
        {
            switch (this)
            {
                case RegNode r: return r.Copy(regValues: r.RegValues.AddOrUpdate(id, value));
                default: return this;
            }
        }

        private Node AddId(TypeTag tag, Id id, Mutation mutation)
        {
            switch (mutation)
            {
                case DeleteM del: return this;
                default:
                    {
                        var pres = GetPres(tag.Key);
                        var presP = pres.AddOrUpdate(id);
                        return SetPres(tag.Key, presP);
                    }
            }
        }

        private (Node, Set<Id>) ClearElem(Set<Id> deps, Key key)
        {
            var (ctx1, pres1) = ClearAny(deps, key);
            var pres2 = ctx1.GetPres(key);
            var pres3 = pres1.Append(pres2).Subtract(deps);
            return (ctx1.SetPres(key, pres3), pres3);
        }

        private (Node, Set<Id>) ClearAny(Set<Id> deps, Key key)
        {
            var ctx0 = this;
            var (ctx1, pres1) = ctx0.Clear(deps, new MapT(key));
            var (ctx2, pres2) = ctx1.Clear(deps, new ListT(key));
            var (ctx3, pres3) = ctx2.Clear(deps, new RegT(key));
            return (ctx3, pres1.Append(pres2).Append(pres3));
        }

        private (Node, Set<Id>) Clear(Set<Id> deps, TypeTag tag)
            => FindChild(tag)
                .Match(
                    Some: node =>
                    {
                        switch (node)
                        {
                            case RegNode rn:
                                {
                                    var concurrent = rn.RegValues.Filter((id, lv) => !deps.Contains(id));
                                    var retNode = AddNode(tag, new RegNode(concurrent));
                                    return (retNode, new Set<Id>(concurrent.Keys));
                                }
                            case MapNode mn:
                                {
                                    var (cleared, pres) = mn.ClearMap(deps, new Set<Key>());
                                    var retNode = AddNode(tag, cleared);
                                    return (retNode, pres);
                                }
                            case ListNode ln:
                                {
                                    var (cleared, pres) = ln.ClearList(deps, new HeadR());
                                    var retNode = AddNode(tag, cleared);
                                    return (retNode, pres);
                                }
                        }

                        throw new InvalidOperationException($"Invalid node type of {node.GetType().Name}");
                    },
                    None: () => (this, new Set<Id>()));

        private (Node, Set<Id>) ClearMap(Set<Id> deps, Set<Key> done)
        {
            throw new NotImplementedException();
        }

        private (Node, Set<Id>) ClearList(Set<Id> deps, ListRef @ref)
        {
            throw new NotImplementedException();
        }

        private Node GetChild(TypeTag tag)
            => FindChild(tag).IfNone(() =>
            {
                switch (tag)
                {
                    case MapT m: return Node.EmptyMap;
                    case ListT l: return Node.EmptyList;
                    case RegT r: return Node.EmptyReg;
                    default: throw new InvalidOperationException("Invalid TypeTag.");
                }
            });

        private Option<Node> FindChild(TypeTag tag)
        {
            switch (this)
            {
                case BranchNode n: return n.Children.Find(tag);
                default: return None;
            }
        }

        private Set<Id> GetPres(Key key)
        {
            switch (this)
            {
                case BranchNode n: return n.PresSets.ContainsKey(key) ? n.PresSets[key] : Set<Id>();
                default: return Set<Id>();
            }
        }

        private Node SetPres(Key key, Set<Id> pres)
        {
            switch (this)
            {
                case BranchNode n:
                    {
                        Map<K, Set<V>> RemoveOrUpdate<K, V>(Map<K, Set<V>> map, K k, Set<V> val)
                            where K : class
                        {
                            return (val.Length() == 0)
                                ? map.Remove(k)
                                : map.AddOrUpdate(k, val);
                        }

                        return n.WithPresSets(RemoveOrUpdate(n.PresSets, key, pres));
                    }
                default: return this;
            }
        }

        private ListRef GetPreviousRef(ListRef @ref)
        {
            switch (this)
            {
                case ListNode ln: { return ln.Order.ContainsKey(@ref) ? ln.Order[@ref] : new HeadR(); }
                default: return new HeadR();
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

        private Node SetNextRef(ListRef src, ListRef dst)
        {
            switch (this)
            {
                case ListNode ln:
                {
                    return ln.Copy(order: ln.Order.AddOrUpdate(src, dst));
                }
                default: return this;
            }
        }

        public abstract int CompareTo(object obj);
    }

    public abstract class BranchNode : Node
    {
        public readonly Map<TypeTag, Node> Children;
        public readonly Map<Key, Set<Id>> PresSets;

        public BranchNode(
            Map<TypeTag, Node> children,
            Map<Key, Set<Id>> presSets)
        {
            Children = children;
            PresSets = presSets;
        }

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
            => Copy(children: children);

        public override BranchNode WithPresSets(Map<Key, Set<Id>> presSets)
            => Copy(presSets: presSets);

        public override bool Equals(object other)
            => RecordType<MapNode>.EqualityTyped(this, other as MapNode);

        public override int GetHashCode()
            => RecordType<MapNode>.Hash(this);

        public override int CompareTo(object other)
            => RecordType<MapNode>.Compare(this, other as MapNode);

        public override int CompareTo(Node other)
            => RecordType<MapNode>.Compare(this, other as MapNode);

        // public override bool Equals(object obj)
        // {
        //     if (obj is MapNode mn)
        //     {
        //         return Children.Equals(mn.Children)
        //             && PresSets.Equals(mn.PresSets);
        //     }

        //     return false;
        // }

        // public override int GetHashCode()
        // {
        //     return Children.GetHashCode()
        //          ^ PresSets.GetHashCode();
        // }

        private MapNode Copy(
            Map<TypeTag, Node>? children = null,
            Map<Key, Set<Id>>? presSets = null)
        {
            return new MapNode(
                children ?? Children,
                presSets ?? PresSets);
        }
    }

    public class ListNode : BranchNode
    {
        public readonly Map<ListRef, ListRef> Order;

        /** The tests cannot converge, since the orderArchive of two replicas is
        * always different. Therefore don't compare the orderArchive.
        */
        [OptOutOfEq]
        [OptOutOfOrd]
        public readonly Map<BigInteger, Map<ListRef, ListRef>> OrderArchive;

        public ListNode(
            Map<TypeTag, Node> children,
            Map<Key, Set<Id>> presSets,
            Map<ListRef, ListRef> order,
            Map<BigInteger, Map<ListRef, ListRef>> orderArchive)
            : base(children, presSets)
        {
            Order = order;
            OrderArchive = orderArchive;
        }

        public override BranchNode WithChildren(Map<TypeTag, Node> children)
            => Copy(children: children);

        public override BranchNode WithPresSets(Map<Key, Set<Id>> presSets)
            => Copy(presSets: presSets);

        public override bool Equals(object other)
            => RecordType<ListNode>.EqualityTyped(this, other as ListNode);

        public override int GetHashCode()
            => RecordType<ListNode>.Hash(this);

        public override int CompareTo(object other)
            => RecordType<ListNode>.Compare(this, other as ListNode);

        internal ListNode Copy(
            Map<Key, Set<Id>>? presSets = null,
            Map<TypeTag, Node>? children = null,
            Map<ListRef, ListRef>? order = null,
            Map<BigInteger, Map<ListRef, ListRef>>? orderArchive = null)
            => new ListNode(
                children: children ?? Children,
                presSets: presSets ?? PresSets,
                order: order ?? Order,
                orderArchive: orderArchive ?? OrderArchive
            );
    }

    public class RegNode : Node
    {
        public readonly Map<Id, LeafVal> RegValues;

        public RegNode(Map<Id, LeafVal> regValues)
        {
            RegValues = regValues;
        }

        public Lst<LeafVal> Values()
            => new Lst<LeafVal>(RegValues.Values);

        public RegNode Copy(Map<Id, LeafVal>? regValues = null)
            => new RegNode(regValues ?? RegValues);

        public override bool Equals(object other)
            => RecordType<RegNode>.EqualityTyped(this, other as RegNode);

        public override int GetHashCode()
            => RecordType<RegNode>.Hash(this);

        public override int CompareTo(object other)
            => RecordType<RegNode>.Compare(this, other as RegNode);
    }
}