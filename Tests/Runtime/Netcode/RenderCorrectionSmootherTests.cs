using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class RenderCorrectionSmootherTests
    {
        [Test]
        public void Idle_ReturnsTargetExactly_NoLag()
        {
            var s = new RenderCorrectionSmoother(0.15f);
            Assert.AreEqual(new Vector3(5, 0, 0), s.Smooth(new Vector3(5, 0, 0), 0.016f));
            Assert.AreEqual(new Vector3(9, 0, 3), s.Smooth(new Vector3(9, 0, 3), 0.016f));
        }

        [Test]
        public void AfterCorrection_FirstFrame_NoOvershoot_StaysNearOld()
        {
            var s = new RenderCorrectionSmoother(0.15f);
            s.Smooth(new Vector3(0, 0, 0), 0.016f);   // 추종 → current=old(0)
            s.MarkCorrection(0.3f);
            var r = s.Smooth(new Vector3(2, 0, 0), 0.016f);   // 타깃 +2 점프
            // 버그판(2old-new = -2)이면 실패. 정상: old(0)~target(2) 사이, 반대편(-)로 안 감, 첫 프레임은 old 근처.
            Assert.Greater(r.X, -0.001f, "반대편 오버슈트 금지");
            Assert.Less(r.X, 2f, "타깃 넘지 않음");
            Assert.Less(r.X, 0.5f, "첫 프레임은 old 근처");
        }

        [Test]
        public void DuringWindow_Monotonic_NeverPastTarget()
        {
            var s = new RenderCorrectionSmoother(0.15f);
            s.Smooth(Vector3.Zero, 0.016f);
            s.MarkCorrection(0.3f);
            var target = new Vector3(2, 0, 0);
            float prev = 0f;
            for (int i = 0; i < 20; i++)
            {
                float x = s.Smooth(target, 0.016f).X;
                Assert.GreaterOrEqual(x, prev - 0.001f, "단조 증가");
                Assert.LessOrEqual(x, 2.001f, "타깃 안 넘음");
                prev = x;
            }
            Assert.Greater(prev, 1.5f, "창 동안 대부분 수렴");
        }

        [Test]
        public void WindowExpired_SnapsToTarget()
        {
            var s = new RenderCorrectionSmoother(0.15f);
            s.Smooth(Vector3.Zero, 0.016f);
            s.MarkCorrection(0.1f);
            for (int i = 0; i < 10; i++) { s.Smooth(new Vector3(2, 0, 0), 0.016f); }  // 창(0.1s) 소진
            Assert.AreEqual(new Vector3(7, 0, 0), s.Smooth(new Vector3(7, 0, 0), 0.016f), "만료 후 정확 추종");
        }

        [Test]
        public void Reset_ReinitializesToNextTarget()
        {
            var s = new RenderCorrectionSmoother(0.15f);
            s.Smooth(Vector3.Zero, 0.016f);
            s.MarkCorrection(0.3f);
            s.Smooth(new Vector3(2, 0, 0), 0.016f);
            s.Reset();
            Assert.AreEqual(new Vector3(9, 0, 0), s.Smooth(new Vector3(9, 0, 0), 0.016f), "리셋 후 첫 호출은 타깃으로");
        }
    }
}
