using System;
using FluentAssertions;
using Machine.Specifications;

namespace Mingle
{
    [Subject(typeof(Replica))]
    public class When_apply_cmd
    {
        Establish context = ()
            => Original = Replica.Empty(ReplicaId.New("abc"));

        Because of = ()
            => Subject = Original.ApplyCmd(
                new Expr.Doc().DownField("key").Assign("A"));

        It should_be_a_new_instance = ()
            => ReferenceEquals(Subject, Original).Should().BeFalse();

        static Replica Original;
        static Replica Subject;
    }
}