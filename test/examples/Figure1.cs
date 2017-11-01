using System;
using Xunit;
using FluentAssertions;

namespace Mingle
{
    public class Figure1
    {
        [Fact]
        public void Integration_test()
        {
            var p0 = Replica.Empty(ReplicaId.New("p")).ApplyCmd(new Expr.Doc().DownField("key").Assign("A"));
            var q0 = Merge(Replica.Empty(ReplicaId.New("q")), p0);

            // Assert that p0 and q0 are converged
            Converged(p0, q0);

            var p1 = p0.ApplyCmd(new Expr.Doc().DownField("key").Assign("B"));
            var q1 = q0.ApplyCmd(new Expr.Doc().DownField("key").Assign("C"));

            // Assert that p1 and q1 are diverged
            Diverged(p1, q1);

            var p2 = Merge(p1, q1);
            var q2 = Merge(q1, p1);

            // Assert that p2 and q2 are converged again.
            Converged(p2, q2);
        }

        private void Converged(Replica a, Replica b)
        {
            a.ProcessedOps.ShouldAllBeEquivalentTo(b.ProcessedOps);
            a.Document.ShouldBeEquivalentTo(b.Document);
        }

        private void Converged(Replica a, Replica b, Replica c)
        {
            Converged(a, b);
            Converged(b, c);
        }

        private void Diverged(Replica a, Replica b)
        {
            a.ProcessedOps.Should().NotBeEquivalentTo(b.ProcessedOps);
            a.Document.Should().NotBe(b.Document);
        }

        private Replica Merge(Replica a, Replica b)
            => a.ApplyRemoteOps(b.GeneratedOps);
    }
}