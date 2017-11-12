using System;
using Xunit;
using FluentAssertions;
using LanguageExt;
using static LanguageExt.Prelude;
using Newtonsoft.Json;

namespace Mingle
{
    public class Figure1
    {
        [Fact]
        public void Integration_test()
        {
            var p0 = Replica.Empty(ReplicaId.New("p")).ApplyCmd(new Doc().DownField("key").Assign("A"));
            var q0 = Merge(Replica.Empty(ReplicaId.New("q")), p0);

            // Assert that p0 and q0 are converged
            Converged(p0, q0);

            var p1 = p0.ApplyCmd(new Doc().DownField("key").Assign("B"));
            var q1 = q0.ApplyCmd(new Doc().DownField("key").Assign("C"));

            // Assert that p1 and q1 are diverged
            Diverged(p1, q1);

            var p2 = Merge(p1, q1);
            var q2 = Merge(q1, p1);

            // Assert that p2 and q2 are converged again.
            Converged(p2, q2);
        }

        private static void Converged(Replica a, Replica b)
        {
            Assert.True(a.ProcessedOps == b.ProcessedOps);;
            //a.ProcessedOps.ShouldBeEquivalentTo(b.ProcessedOps);
            //a.Document.Should().Be(b.Document);
            Assert.True(a.Document == b.Document);
        }

        private static void Converged(Replica a, Replica b, Replica c)
        {
            Converged(a, b);
            Converged(b, c);
        }

        private static void Diverged(Replica a, Replica b)
        {
            Assert.True(a.ProcessedOps != b.ProcessedOps);
            //a.ProcessedOps.Should().NotBeEquivalentTo(b.ProcessedOps);
            Assert.True(a.Document != b.Document);
            //a.Document.Should().NotBe(b.Document);
        }

        private static Replica Merge(Replica a, Replica b)
            => a.ApplyRemoteOps(b.GeneratedOps);

        [Fact]
        public void TestMapEquality()
        {
            var a = Map((1, "Rod"),
                     (2, "Jane"),
                     (3, "Freddy"));
            var b = Map((1, "Rod"),
                     (2, "Jane"),
                     (3, "Freddy"));

            Assert.True(a == b);
        }
    }
}