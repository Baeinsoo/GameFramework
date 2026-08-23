using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class CapsuleShapeTests
    {
        [Test]
        public void AttachesToEntityAndRoundTrips()
        {
            var entity = new Entity("e1");
            entity.Add(new CapsuleShape(0.45f, 0.9f));

            Assert.AreEqual(0.45f, entity.Get<CapsuleShape>().Radius, 1e-4f);
            Assert.AreEqual(0.9f, entity.Get<CapsuleShape>().Height, 1e-4f);
            Assert.AreSame(entity, entity.Get<CapsuleShape>().Owner);
        }

        [Test]
        public void RejectsNonPositiveSize()
        {
            // 0이나 음수 캡슐은 sweep이 아무것도 못 맞히는 몸이 된다 — 만들 때 막는다.
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new CapsuleShape(0f, 1f));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new CapsuleShape(0.5f, -1f));
        }
    }
}
