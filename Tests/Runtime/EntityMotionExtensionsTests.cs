using NUnit.Framework;
using UnityEngine;
using GameFramework.World;

namespace GameFramework.Tests
{
    public class EntityMotionExtensionsTests
    {
        private static Entity MakeEntity()
        {
            var e = new Entity("test-1");
            e.Add(new GameFramework.World.Transform());
            e.Add(new Velocity());
            return e;
        }

        [Test]
        public void SetPosition_then_GetPosition_roundtrips()
        {
            var e = MakeEntity();
            e.SetPosition(new Vector3(1f, 2f, 3f));
            Assert.AreEqual(new Vector3(1f, 2f, 3f), e.GetPosition());
            // 진실원본(numerics)에도 반영됐는지 확인
            Assert.AreEqual(1f, e.Get<GameFramework.World.Transform>().Position.X, 1e-4f);
        }

        [Test]
        public void SetVelocity_then_GetVelocity_roundtrips()
        {
            var e = MakeEntity();
            e.SetVelocity(new Vector3(-4f, 0f, 5f));
            Assert.AreEqual(new Vector3(-4f, 0f, 5f), e.GetVelocity());
        }

        [Test]
        public void SetRotation_writes_quaternion_and_GetRotation_returns_euler()
        {
            var e = MakeEntity();
            e.SetRotation(new Vector3(0f, 90f, 0f));
            // GetRotation은 eulerAngles(래핑 있음) → y≈90 확인
            Assert.AreEqual(90f, e.GetRotation().y, 0.5f);
        }

        [Test]
        public void SetPosition_with_equal_value_is_noop_on_reference()
        {
            var e = MakeEntity();
            e.SetPosition(new Vector3(1f, 2f, 3f));
            var before = e.Get<GameFramework.World.Transform>().Position;
            e.SetPosition(new Vector3(1f, 2f, 3f)); // 동일값 → 가드로 스킵
            Assert.AreEqual(before, e.Get<GameFramework.World.Transform>().Position);
        }
    }
}
