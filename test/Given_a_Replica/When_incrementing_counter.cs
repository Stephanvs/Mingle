using System;
using FluentAssertions;
using LanguageExt;
using Machine.Specifications;
using Xunit;

namespace Mingle
{
    [Subject(typeof(Replica))]
    public class When_incrementing_counter
    {
        Establish context = ()
            => Empty = Replica.Empty(ReplicaId.New("abc"));

        Because of = ()
            => Subject = When_incrementing_counter.Empty.IncrementCounter();

        It should_be_a_new_instance = ()
            => Subject.Should().NotBeSameAs(Empty);

        It should_have_increased_the_counter = ()
            => Subject.OpsCounter.ShouldBeEquivalentTo(bigint.One);

        static Replica Empty;
        static Replica Subject;
    }
}