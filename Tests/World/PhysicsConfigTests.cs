using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class PhysicsConfigTests
    {
        [Test]
        public void AttachesToEntityAndRoundTrips()
        {
            var entity = new Entity("e1");
            entity.Add(new PhysicsConfig(BodyKind.Kinematic, freezeRotation: true, isTrigger: false));

            var config = entity.Get<PhysicsConfig>();
            Assert.AreEqual(BodyKind.Kinematic, config.Kind);
            Assert.IsTrue(config.FreezeRotation);
            Assert.IsFalse(config.IsTrigger);
            Assert.AreSame(entity, config.Owner);
        }

        [Test]
        public void DynamicBodyMeansPhysicsOwnsMotion()
        {
            // 이 파생 속성이 "밀어넣을 것인가 읽어올 것인가"를 가른다.
            Assert.IsTrue(new PhysicsConfig(BodyKind.Dynamic, false, false).PhysicsOwnsMotion);
        }

        [Test]
        public void NonDynamicBodiesAreDrivenByUs()
        {
            // Static도 PhysX가 움직이지 않는다 — 우리가 밀어넣는 쪽이다.
            Assert.IsFalse(new PhysicsConfig(BodyKind.Kinematic, true, false).PhysicsOwnsMotion);
            Assert.IsFalse(new PhysicsConfig(BodyKind.Static, true, false).PhysicsOwnsMotion);
        }
    }
}
