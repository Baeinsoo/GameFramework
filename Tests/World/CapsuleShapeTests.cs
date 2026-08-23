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

        [Test]
        public void RejectsHeightShorterThanDiameter()
        {
            // 높이가 지름보다 낮으면 Unity CapsuleCollider는 구로 찌그러뜨리는데(height를 2*radius로 클램프),
            // KinematicMover.Cast는 그대로 계산해 위아래가 뒤집힌 캡슐을 만든다 — sweep과 콜라이더가 다시 갈린다.
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new CapsuleShape(0.5f, 0.9f));

            // 경계값(높이 == 지름)은 통과해야 한다 — 실제 튜닝값(radius=0.45, height=0.9)이 바로 이 경계라,
            // 여기서 실수로 <=를 쓰면 지금 마스터데이터와 위 라운드트립 테스트가 깨진다.
            Assert.DoesNotThrow(() => new CapsuleShape(0.45f, 0.9f));
        }
    }
}
