using System.Collections.Generic;
using System.Numerics;
using LanguageExt;
using static LanguageExt.Prelude;
using Xunit;

namespace Mingle.Tests
{
    public class Sanity
    {
        [Theory]
        [MemberData(nameof(GetEqualityCheckData))]
        public void Check_Equality(object left, object right)
        {
            Assert.Equal(left, right);
        }

        public static IEnumerable<object[]> GetEqualityCheckData()
        {
            yield return new []
            {
                Replica.Empty(ReplicaId.New("abc")),
                Replica.Empty(ReplicaId.New("abc"))
            };
            yield return new []
            {
                new Id(1, ReplicaId.New("a")),
                new Id(1, ReplicaId.New("a"))
            };
            yield return new []
            {
                new StrK("abc"),
                new StrK("abc")
            };
            yield return new []
            {
                new IdK(new Id(1, ReplicaId.New("a"))),
                new IdK(new Id(1, ReplicaId.New("a")))
            };
            yield return new []
            {
                Node.EmptyMap,
                Node.EmptyMap
            };
            yield return new []
            {
                new DocK(),
                new DocK()
            };
            yield return new []
            {
                new HeadK(),
                new HeadK()
            };
            yield return new []
            {
                new Var("abc"),
                new Var("abc")
            };
            yield return new []
            {
                new DownField(new Doc(), "key"),
                new DownField(new Doc(), "key"),
            };
            yield return new []
            {
                new AssignM(new Str("abc")),
                new AssignM(new Str("abc"))
            };
            yield return new []
            {
                new DeleteM(),
                new DeleteM()
            };
            yield return new []
            {
                new Doc().DownField("key").Assign("A"),
                new Doc().DownField("key").Assign("A")
            };
            yield return new []
            {
                Replica.Empty(ReplicaId.New("p")).ApplyCmd(new Doc().DownField("key").Assign("A")),
                Replica.Empty(ReplicaId.New("p")).ApplyCmd(new Doc().DownField("key").Assign("A"))
            };
            yield return new Operation[]
            {
                new Operation(
                    new Id(1, ReplicaId.New("a")),
                    LanguageExt.Set<Id>.Empty,
                    Cursor.Doc(),
                    new AssignM(new Str("abc"))
                ),
                new Operation(
                    new Id(1, ReplicaId.New("a")),
                    LanguageExt.Set<Id>.Empty,
                    Cursor.Doc(),
                    new AssignM(new Str("abc"))
                )
            };
            yield return new []
            {
                new RegNode(LanguageExt.Prelude.Map<Id, LeafVal>((
                    new Id(1, ReplicaId.New("abc")),
                    new Str("abc")
                ))),
                new RegNode(LanguageExt.Prelude.Map<Id, LeafVal>((
                    new Id(1, ReplicaId.New("abc")),
                    new Str("adfs")
                )))
            };
        }
    }
}