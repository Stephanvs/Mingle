using Xunit;

namespace Mingle.Tests
{
    public class SyntaxSpecs
    {
        [Fact]
        public void Assign()
        {
            var actual = new Var("list").Assign(new EmptyList());
            var expected = new Assign(new Var("list"), new EmptyList());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Insert()
        {
            var actual = new Doc().DownField("key").Iter().Insert(new Null());
            var expected = new Insert(new Iter(new DownField(new Doc(), "key")), new Null());

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Delete()
        {
            var actual = new Doc().Delete();
            var expected = new Delete(new Doc());

            Assert.Equal(expected, actual);
        }
    }
}