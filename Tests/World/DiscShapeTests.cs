using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class DiscShapeTests
    {
        [Test]
        public void AttachesToEntityAndRoundTrips()
        {
            var entity = new Entity("coin1");
            entity.Add(new DiscShape(0.15f, 0.02f));

            Assert.AreEqual(0.15f, entity.Get<DiscShape>().Radius, 1e-4f);
            Assert.AreEqual(0.02f, entity.Get<DiscShape>().Thickness, 1e-4f);
            Assert.AreSame(entity, entity.Get<DiscShape>().Owner);
        }

        [Test]
        public void RejectsNonPositiveSize()
        {
            // 0이나 음수 원반은 콜라이더가 아무것도 못 맞히는 몸이 된다 — 만들 때 막는다.
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new DiscShape(0f, 0.02f));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new DiscShape(0.15f, -0.02f));
        }
    }
}
