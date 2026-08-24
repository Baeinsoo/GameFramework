using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class SnapshotExtrapolationTests
    {
        [Test]
        public void 경과가_0이면_그_자리다()
        {
            Vector3 result = SnapshotExtrapolation.Position(
                new Vector3(1f, 2f, 3f), Vector3.One, new Vector3(0f, -10f, 0f), 0f, 0.25f);

            Assert.AreEqual(new Vector3(1f, 2f, 3f), result);
        }

        [Test]
        public void 가속도가_없으면_등속으로_나간다()
        {
            Vector3 result = SnapshotExtrapolation.Position(
                Vector3.Zero, new Vector3(2f, 0f, 0f), Vector3.Zero, 0.1f, 0.25f);

            Assert.AreEqual(0.2f, result.X, 0.0001f);
        }

        [Test]
        public void 중력을_포함해_포물선을_그린다()
        {
            // y = v*t + 0.5*a*t^2 = 5*0.1 + 0.5*(-20)*0.01 = 0.5 - 0.1 = 0.4
            Vector3 result = SnapshotExtrapolation.Position(
                Vector3.Zero, new Vector3(0f, 5f, 0f), new Vector3(0f, -20f, 0f), 0.1f, 0.25f);

            Assert.AreEqual(0.4f, result.Y, 0.0001f);
        }

        [Test]
        public void 상한을_넘으면_상한에서_멈춘다()
        {
            Vector3 atCap = SnapshotExtrapolation.Position(
                Vector3.Zero, new Vector3(2f, 0f, 0f), Vector3.Zero, 0.25f, 0.25f);
            Vector3 beyond = SnapshotExtrapolation.Position(
                Vector3.Zero, new Vector3(2f, 0f, 0f), Vector3.Zero, 5f, 0.25f);

            Assert.AreEqual(atCap, beyond);
        }

        [Test]
        public void 경과가_음수면_그_자리다()
        {
            Vector3 result = SnapshotExtrapolation.Position(
                new Vector3(1f, 0f, 0f), Vector3.One, Vector3.Zero, -1f, 0.25f);

            Assert.AreEqual(new Vector3(1f, 0f, 0f), result);
        }

        [Test]
        public void 속도_경과가_0이면_원래_속도다()
        {
            Vector3 result = SnapshotExtrapolation.Velocity(
                new Vector3(0f, 5f, 0f), new Vector3(0f, -10f, 0f), 0f, 0.25f);

            Assert.AreEqual(new Vector3(0f, 5f, 0f), result);
        }

        [Test]
        public void 속도_가속도가_없으면_그대로_유지된다()
        {
            Vector3 result = SnapshotExtrapolation.Velocity(
                new Vector3(2f, 0f, 0f), Vector3.Zero, 0.1f, 0.25f);

            Assert.AreEqual(new Vector3(2f, 0f, 0f), result);
        }

        [Test]
        public void 속도_중력이_시간만큼_더해진다()
        {
            // v = v0 + a*t = 5 + (-20)*0.1 = 3
            Vector3 result = SnapshotExtrapolation.Velocity(
                new Vector3(0f, 5f, 0f), new Vector3(0f, -20f, 0f), 0.1f, 0.25f);

            Assert.AreEqual(3f, result.Y, 0.0001f);
        }

        [Test]
        public void 속도_상한을_넘으면_상한에서_멈춘다()
        {
            Vector3 atCap = SnapshotExtrapolation.Velocity(
                Vector3.Zero, new Vector3(0f, -10f, 0f), 0.25f, 0.25f);
            Vector3 beyond = SnapshotExtrapolation.Velocity(
                Vector3.Zero, new Vector3(0f, -10f, 0f), 5f, 0.25f);

            Assert.AreEqual(atCap, beyond);
        }

        [Test]
        public void 속도_경과가_음수면_원래_속도다()
        {
            Vector3 result = SnapshotExtrapolation.Velocity(
                new Vector3(1f, 0f, 0f), new Vector3(0f, -10f, 0f), -1f, 0.25f);

            Assert.AreEqual(new Vector3(1f, 0f, 0f), result);
        }

        [Test]
        public void 위치와_속도가_같은_순간을_말한다()
        {
            // 위치식 p(t)=p0+v0t+0.5at^2 을 미분하면 v(t)=v0+at — 두 반환값이 같은 물리 상태를
            // 말하는지(둘 다 클램프된 같은 t를 쓰는지) 상한 너머 경과로 교차검증한다.
            Vector3 v0 = new Vector3(0f, 8f, 0f);
            Vector3 a = new Vector3(0f, -20f, 0f);
            float maxElapsed = 0.25f;

            Vector3 vAtCap = SnapshotExtrapolation.Velocity(v0, a, maxElapsed, maxElapsed);
            Vector3 vBeyond = SnapshotExtrapolation.Velocity(v0, a, 10f, maxElapsed);

            Assert.AreEqual(vAtCap, vBeyond);
            Assert.AreEqual(v0.Y + a.Y * maxElapsed, vAtCap.Y, 0.0001f);
        }
    }
}
