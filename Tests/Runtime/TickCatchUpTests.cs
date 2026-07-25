using NUnit.Framework;
using GameFramework.Runner;

namespace GameFramework.Tests
{
    public class TickCatchUpTests
    {
        [Test]
        public void NothingDue_ReturnsBelowTick_SoLoopSkips()
        {
            Assert.AreEqual(4, TickCatchUp.ClampTarget(tick: 5, processibleTick: 4, maxTicksPerFrame: 8));
        }

        [Test]
        public void CaughtUp_ReturnsProcessibleTick()
        {
            Assert.AreEqual(5, TickCatchUp.ClampTarget(5, 5, 8));
        }

        [Test]
        public void BehindWithinCap_ReturnsProcessibleTick()
        {
            Assert.AreEqual(3, TickCatchUp.ClampTarget(0, 3, 8));
        }

        [Test]
        public void BehindBeyondCap_ClampsToCap()
        {
            Assert.AreEqual(7, TickCatchUp.ClampTarget(0, 100, 8));
        }

        [Test]
        public void ExactlyAtCapBoundary_ClampsOneShortOfProcessible()
        {
            Assert.AreEqual(7, TickCatchUp.ClampTarget(0, 8, 8));
        }

        [Test]
        public void CapBelowOne_TreatedAsOne()
        {
            Assert.AreEqual(0, TickCatchUp.ClampTarget(0, 100, 0));
        }
    }
}
