using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class HermiteTests
    {
        [Test]
        public void U0_ReturnsStart_IgnoringVelocities()
        {
            var p0 = new Vector3(1, 2, 3);
            var r = Hermite.Position(p0, new Vector3(9, 9, 9), new Vector3(5, 5, 5), new Vector3(-9, 0, 0), 0.033f, 0f);
            Assert.AreEqual(p0.X, r.X, 1e-5f); Assert.AreEqual(p0.Y, r.Y, 1e-5f); Assert.AreEqual(p0.Z, r.Z, 1e-5f);
        }

        [Test]
        public void U1_ReturnsEnd_IgnoringVelocities()
        {
            var p1 = new Vector3(5, 5, 5);
            var r = Hermite.Position(new Vector3(1, 2, 3), new Vector3(9, 9, 9), p1, new Vector3(-9, 0, 0), 0.033f, 1f);
            Assert.AreEqual(p1.X, r.X, 1e-5f); Assert.AreEqual(p1.Y, r.Y, 1e-5f); Assert.AreEqual(p1.Z, r.Z, 1e-5f);
        }

        [Test]
        public void ZeroVelocities_EasesLikeSmoothstep()
        {
            // 끝점 속도 0 → 큐빅 Hermite는 smoothstep(ease-in-out), 선형 Lerp 아님.
            var p0 = new Vector3(0, 0, 0);
            var p1 = new Vector3(10, 0, 0);
            Assert.AreEqual(5.0f, Hermite.Position(p0, Vector3.Zero, p1, Vector3.Zero, 0.033f, 0.5f).X, 1e-5f);
            Assert.AreEqual(1.5625f, Hermite.Position(p0, Vector3.Zero, p1, Vector3.Zero, 0.033f, 0.25f).X, 1e-5f);
        }

        [Test]
        public void ConstantVelocity_TravelsStraightAtConstantSpeed()
        {
            float dt = 0.033f;
            var v = new Vector3(3, 0, 0);
            var p0 = new Vector3(0, 0, 0);
            var p1 = p0 + v * dt;
            var r = Hermite.Position(p0, v, p1, v, dt, 0.5f);
            Assert.AreEqual(0.5f * dt * 3f, r.X, 1e-5f);
        }
    }
}
