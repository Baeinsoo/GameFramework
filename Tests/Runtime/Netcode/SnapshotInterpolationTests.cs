using System.Collections.Generic;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class SnapshotInterpolationTests
    {
        private static readonly IReadOnlyList<double> Times = new double[] { 1.0, 2.0, 3.0, 4.0 };

        [Test]
        public void Midpoint_BracketsPair_WithHalfAlpha()
        {
            var r = SnapshotInterpolation.Solve(Times, 2.5);
            Assert.AreEqual(1, r.Lower);
            Assert.AreEqual(2, r.Upper);
            Assert.AreEqual(0.5f, r.Alpha, 1e-5f);
        }

        [Test]
        public void QuarterPoint_ComputesAlpha()
        {
            var r = SnapshotInterpolation.Solve(Times, 3.25);   // 25% from 3.0 to 4.0
            Assert.AreEqual(2, r.Lower);
            Assert.AreEqual(3, r.Upper);
            Assert.AreEqual(0.25f, r.Alpha, 1e-5f);
        }

        [Test]
        public void ExactLowerBoundary_AlphaZero()
        {
            var r = SnapshotInterpolation.Solve(Times, 2.0);
            Assert.AreEqual(1, r.Lower);
            Assert.AreEqual(2, r.Upper);
            Assert.AreEqual(0f, r.Alpha, 1e-5f);
        }

        [Test]
        public void BeforeOldest_HoldsOldest()
        {
            var r = SnapshotInterpolation.Solve(Times, 0.3);
            Assert.AreEqual(0, r.Lower);
            Assert.AreEqual(0, r.Upper);   // Lower==Upper → caller lerps a point onto itself = hold
            Assert.AreEqual(0f, r.Alpha, 1e-5f);
        }

        [Test]
        public void AfterNewest_HoldsNewest()
        {
            var r = SnapshotInterpolation.Solve(Times, 9.0);
            Assert.AreEqual(3, r.Lower);
            Assert.AreEqual(3, r.Upper);   // last index held
            Assert.AreEqual(0f, r.Alpha, 1e-5f);
        }

        [Test]
        public void SingleSample_HoldsIt()
        {
            var r = SnapshotInterpolation.Solve(new double[] { 5.0 }, 5.0);
            Assert.AreEqual(0, r.Lower);
            Assert.AreEqual(0, r.Upper);
            Assert.AreEqual(0f, r.Alpha, 1e-5f);
        }

        [Test]
        public void Empty_ReturnsInvalid()
        {
            var r = SnapshotInterpolation.Solve(new double[0], 1.0);
            Assert.AreEqual(-1, r.Lower);
            Assert.AreEqual(-1, r.Upper);
        }
    }
}
