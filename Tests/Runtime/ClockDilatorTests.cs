using NUnit.Framework;
using GameFramework.Netcode;

namespace GameFramework.Tests
{
    public class ClockDilatorTests
    {
        [Test]
        public void Advance_ErrorBeyondSnapThreshold_SnapsToTarget()
        {
            var dilator = new ClockDilator(maxRate: 0.05, errorScale: 0.1, snapThreshold: 0.5);
            double result = dilator.Advance(current: 0.0, target: 1.0, realDelta: 0.016);
            Assert.AreEqual(1.0, result, 1e-9);
        }

        [Test]
        public void Advance_TargetAhead_SaturatesToMaxRate()
        {
            var dilator = new ClockDilator(0.05, 0.1, 0.5);
            double result = dilator.Advance(0.0, 0.4, 0.016);
            Assert.AreEqual(0.016 * 1.05, result, 1e-9);
            Assert.Greater(result, 0.016);
        }

        [Test]
        public void Advance_TargetBehind_DeceleratesButNeverRewinds()
        {
            var dilator = new ClockDilator(0.05, 0.1, 0.5);
            double result = dilator.Advance(1.0, 0.95, 0.016);
            Assert.AreEqual(1.0 + 0.016 * 0.95, result, 1e-9);
            Assert.Greater(result, 1.0);
            Assert.Less(result, 1.0 + 0.016);
        }

        [Test]
        public void Advance_SmallError_ProportionalDilation()
        {
            var dilator = new ClockDilator(0.05, 1.0, 0.5);
            double result = dilator.Advance(0.0, 0.025, 0.016);
            Assert.AreEqual(0.016 * 1.025, result, 1e-9);
        }
    }
}
