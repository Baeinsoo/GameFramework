using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class HermiteVelocityTests
    {
        [Test]
        public void U0_ReturnsStartVelocity()
        {
            var v0 = new Vector3(3, -1, 2);
            var r = Hermite.Velocity(new Vector3(1, 2, 3), v0, new Vector3(5, 5, 5), new Vector3(-9, 0, 0), 0.05f, 0f);
            Assert.AreEqual(v0.X, r.X, 1e-4f); Assert.AreEqual(v0.Y, r.Y, 1e-4f); Assert.AreEqual(v0.Z, r.Z, 1e-4f);
        }

        [Test]
        public void U1_ReturnsEndVelocity()
        {
            var v1 = new Vector3(-9, 4, 0);
            var r = Hermite.Velocity(new Vector3(1, 2, 3), new Vector3(3, -1, 2), new Vector3(5, 5, 5), v1, 0.05f, 1f);
            Assert.AreEqual(v1.X, r.X, 1e-4f); Assert.AreEqual(v1.Y, r.Y, 1e-4f); Assert.AreEqual(v1.Z, r.Z, 1e-4f);
        }

        [Test]
        public void ConstantVelocity_StaysConstantThroughout()
        {
            // 등속 직선 구간이면 곡선이 직선이므로 어느 지점에서든 같은 속도가 나와야 한다.
            float dt = 0.05f;
            var v = new Vector3(3, 0, 0);
            var p0 = new Vector3(0, 0, 0);
            var p1 = p0 + v * dt;
            foreach (float u in new[] { 0f, 0.25f, 0.5f, 0.75f, 1f })
            {
                var r = Hermite.Velocity(p0, v, p1, v, dt, u);
                Assert.AreEqual(3f, r.X, 1e-4f, "u=" + u);
            }
        }

        [Test]
        public void ZeroEndpointVelocities_PeaksAtMidpoint()
        {
            // 양 끝 속도 0 + 이동 있음 → smoothstep. 속도는 중간에서 최대(= 1.5 × 평균속도).
            float dt = 0.5f;
            var p0 = new Vector3(0, 0, 0);
            var p1 = new Vector3(10, 0, 0);
            float average = 10f / dt;

            Assert.AreEqual(0f, Hermite.Velocity(p0, Vector3.Zero, p1, Vector3.Zero, dt, 0f).X, 1e-4f);
            Assert.AreEqual(0f, Hermite.Velocity(p0, Vector3.Zero, p1, Vector3.Zero, dt, 1f).X, 1e-4f);
            Assert.AreEqual(1.5f * average, Hermite.Velocity(p0, Vector3.Zero, p1, Vector3.Zero, dt, 0.5f).X, 1e-3f);
        }

        [Test]
        public void MatchesNumericalDerivativeOfPosition()
        {
            // 위치 곡선의 실제 기울기와 일치해야 한다 — 이게 lerp 대신 미분을 쓰는 이유다.
            float dt = 0.05f;
            var p0 = new Vector3(0, 0, 0);
            var v0 = new Vector3(2, 0, 0);
            var p1 = new Vector3(1, 1, 0);
            var v1 = new Vector3(0, 5, 0);

            const float h = 1e-4f;
            float u = 0.37f;
            Vector3 ahead = Hermite.Position(p0, v0, p1, v1, dt, u + h);
            Vector3 behind = Hermite.Position(p0, v0, p1, v1, dt, u - h);
            Vector3 numeric = (ahead - behind) / (2f * h * dt);   // du → 초 단위로 환산

            Vector3 analytic = Hermite.Velocity(p0, v0, p1, v1, dt, u);
            Assert.AreEqual(numeric.X, analytic.X, 1e-2f);
            Assert.AreEqual(numeric.Y, analytic.Y, 1e-2f);
        }

        [Test]
        public void NonPositiveInterval_FallsBackToStartVelocity()
        {
            var v0 = new Vector3(7, 0, 0);
            var r = Hermite.Velocity(new Vector3(1, 0, 0), v0, new Vector3(2, 0, 0), new Vector3(9, 9, 9), 0f, 0.5f);
            Assert.AreEqual(v0.X, r.X, 1e-4f);
        }
    }
}
