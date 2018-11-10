using Xunit;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Mingle.Tests
{
    public class ReplicaSpecs
    {
        [Fact]
        public void EvalExpr_with_empty_doc_returns_cursor_with_finalKey_of_DocK()
        {
            var p0 = Replica.Empty("p");
            var cursor = p0.EvalExpr(new Doc());
            var expected = Cursor.WithFinalKey(new DocK());
            Assert.Equal(expected, cursor);
        }

        [Fact]
        public void EvalExpr_with_DownField_returns_cursor_with_correct_branchtag_and_key()
        {
            var p0 = Replica.Empty("p");
            var cursor = p0.EvalExpr(new Doc().DownField("key"));
            var expected = new Cursor(List<BranchTag>(new MapT(new DocK())), new StrK("key"));
            Assert.Equal(expected, cursor);
        }

        [Fact]
        public void EvalExpr_with_Iter_returns_cursor_with_correct_structure()
        {
            var p0 = Replica.Empty("p");
            var cursor = p0.EvalExpr(new Doc().Iter());
            var expected = new Cursor(List<BranchTag>(new ListT(new DocK())), new HeadK());
            Assert.Equal(expected, cursor);
        }

        [Fact]
        public void EvalExpr_with_DownField_and_Iter_returns_cursor_with_correct_structure()
        {
            var p0 = Replica.Empty("p");
            var cursor = p0.EvalExpr(new Doc().DownField("key").Iter());
            var expected = new Cursor(List<BranchTag>(new MapT(new DocK()), new ListT(new StrK("key"))), new HeadK());
            Assert.Equal(expected, cursor);
        }

        [Fact]
        public void EvalExpr_with_Iter_and_Next_returns_cursor_with_correct_structure()
        {
            var p0 = Replica.Empty("p");
            var cursor = p0.EvalExpr(new Doc().Iter().Next());
            var expected = new Cursor(List<BranchTag>(new ListT(new DocK())), new HeadK());
            Assert.Equal(expected, cursor);
        }

        [Fact]
        public void EvalExpr_with_DownField_and_Iter_and_Next_returns_cursor_with_correct_structure()
        {
            // property("list.iter.next") = secure {
            //     val list = doc.downField("list")
            //     val cmd = (list := `[]`) `;`
            //         list.iter.insert("item1") `;`
            //         list.iter.insert("item2") `;`
            //         list.iter.insert("item3")

            //     val p1 = p0.applyCmd(cmd)
            //     val e1 = list.iter.next
            //     val e2 = list.iter.next.next
            //     val e3 = list.iter.next.next.next
            //     val cur = p1.evalExpr(list.iter)

            //     (p1.evalExpr(e1) ?= cur.copy(finalKey = IdK(Id(4, "p")))) &&
            //     (p1.evalExpr(e2) ?= cur.copy(finalKey = IdK(Id(3, "p")))) &&
            //     (p1.evalExpr(e3) ?= cur.copy(finalKey = IdK(Id(2, "p"))))
            // }

            var list = new Doc().DownField("list");
            var cmd = list.Assign(new EmptyList())
                .Append(list.Iter().Insert("item1"))
                .Append(list.Iter().Insert("item2"))
                .Append(list.Iter().Insert("item3"));

            var p0 = Replica.Empty("p");
            var p1 = p0.ApplyCmd(cmd);
            var e1 = list.Iter().Next();
            var e2 = list.Iter().Next().Next();
            var e3 = list.Iter().Next().Next().Next();
            var cur = p1.EvalExpr(list.Iter());

            Assert.Equal(cur.Copy(finalKey: new IdK(4, "p")), p1.EvalExpr(e1));
            Assert.Equal(cur.Copy(finalKey: new IdK(3, "p")), p1.EvalExpr(e2));
            Assert.Equal(cur.Copy(finalKey: new IdK(2, "p")), p1.EvalExpr(e3));
        }
    }
}