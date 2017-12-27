using FluentAssertions;
using LanguageExt;
using Machine.Specifications;

namespace Mingle.Tests
{
    [Subject(typeof(Replica))]
    public class When_empty
    {
        Because of = ()
            => Subject = Replica.Empty(ReplicaId.New("1234"));

        It should_have_unique_replicaId = ()
            => Subject.ReplicaId.Should().Be(ReplicaId.New("1234"));

        It should_start_with_opsCounter_of_zero = ()
            => Subject.CurrentId.OpsCounter.ShouldBeEquivalentTo(bigint.Zero);

        static Replica Subject;
    }
}