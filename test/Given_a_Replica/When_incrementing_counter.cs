using System.Numerics;
using FluentAssertions;
using LanguageExt;
using Machine.Specifications;

namespace Mingle
{
    [Subject(typeof(Replica))]
    public class When_incrementing_counter
    {
        Establish context = ()
            => Empty = Replica.Empty(ReplicaId.New("abc"));

        Because of = ()
            => Subject = Empty.IncrementCounter();

        It should_be_a_new_instance = ()
            => Subject.Should().NotBeSameAs(Empty);

        It should_have_increased_the_counter = ()
            => Subject.OpsCounter.ShouldBeEquivalentTo(BigInteger.One);

        static Replica Empty;
        static Replica Subject;
    }
}