using System;
using FluentAssertions;
using Machine.Specifications;

namespace Mingle
{
    [Subject(typeof(Replica))]
    public class When_apply_cmd
    {
        Establish context = ()
            => Empty = Replica.Empty(ReplicaId.New("abc"));

        Because of = ()
            => Subject = Empty.ApplyCmd(
                new Doc().DownField("key").Assign("A"));

        It should_be_a_new_instance = ()
            => ReferenceEquals(Subject, Empty).Should().BeFalse();

        static Replica Empty;
        static Replica Subject;
    }
}