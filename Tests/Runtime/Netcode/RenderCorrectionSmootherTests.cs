using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class RenderCorrectionSmootherTests
    {
        // 보정이 없으면 렌더는 sim을 정확히 따른다(지연 0).
        [Test]
        public void NoCorrection_TargetEqualsSim()
        {
            var s = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            Assert.AreEqual(new Vector3(5, 0, 0), s.Target(new Vector3(5, 0, 0)));
            s.DecayTick(0.033f);
            Assert.AreEqual(new Vector3(9, 0, 3), s.Target(new Vector3(9, 0, 3)));
        }

        // 핵심(번쩍 회귀 방지): 보정 직후 첫 렌더는 '있던 자리(old)'에 머문다 — 반대편 오버슈트(2old-new) 아님.
        [Test]
        public void OnCorrection_FirstTarget_StaysAtOld_NoOvershoot()
        {
            var s = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            s.Target(new Vector3(0, 0, 0));                       // _lastTarget = old(0)
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0)); // 0.6m 보정(밴드 내)
            var r = s.Target(new Vector3(0.6f, 0, 0));            // 감쇠 전이므로 정확히 old
            Assert.AreEqual(0f, r.X, 1e-4f, "보정 직후 렌더는 old(0)에 유지 — 버그판 -0.6(오버슈트) 아님");
        }

        // 보정 후 감쇠하면 렌더가 단조로 새 위치에 수렴하며 넘지 않는다.
        [Test]
        public void AfterCorrection_Decay_MonotonicConvergeToNew_NeverPast()
        {
            var s = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0));
            var target = new Vector3(0.6f, 0, 0);
            float prev = s.Target(target).X;                     // 0
            for (int i = 0; i < 30; i++)
            {
                s.DecayTick(0.033f);
                float x = s.Target(target).X;
                Assert.GreaterOrEqual(x, prev - 1e-4f, "단조 증가");
                Assert.LessOrEqual(x, 0.6f + 1e-4f, "새 위치를 넘지 않음");
                prev = x;
            }
            Assert.Greater(prev, 0.55f, "충분히 감쇠하면 새 위치에 근접");
        }

        // 무시할 만큼 작은 보정(< min)은 offset 0 → 즉시 새 위치 채택.
        [Test]
        public void SmallCorrection_BelowMin_AdoptsImmediately()
        {
            var s = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.01f, 0, 0)); // 0.01m < 0.025
            Assert.AreEqual(new Vector3(0.01f, 0, 0), s.Target(new Vector3(0.01f, 0, 0)));
        }

        // 너무 큰 보정(> teleport, 리스폰 등)은 offset 0 → 즉시 스냅.
        [Test]
        public void LargeCorrection_AboveTeleport_Snaps()
        {
            var s = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(5, 0, 0)); // 5m > 3
            Assert.AreEqual(new Vector3(5, 0, 0), s.Target(new Vector3(5, 0, 0)));
        }

        // 같은 총 감쇠시간이면 dt 분할이 달라도 결과가 근사 동일(프레임독립).
        [Test]
        public void Decay_FrameRateIndependent()
        {
            var target = new Vector3(1, 0, 0);

            var coarse = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            coarse.Target(new Vector3(0, 0, 0));
            coarse.OnCorrection(new Vector3(0, 0, 0), target);
            for (int i = 0; i < 5; i++) coarse.DecayTick(0.02f);   // 5 × 0.02 = 0.1s

            var fine = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            fine.Target(new Vector3(0, 0, 0));
            fine.OnCorrection(new Vector3(0, 0, 0), target);
            for (int i = 0; i < 10; i++) fine.DecayTick(0.01f);    // 10 × 0.01 = 0.1s

            Assert.AreEqual(coarse.Target(target).X, fine.Target(target).X, 1e-4f);
        }

        // Reset 후 offset이 0으로 초기화되어 sim을 정확히 따른다.
        [Test]
        public void Reset_ClearsOffset()
        {
            var s = new RenderCorrectionSmoother(0.1f, 0.025f, 3f);
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0));
            s.Target(new Vector3(0.6f, 0, 0));
            s.Reset();
            Assert.AreEqual(new Vector3(9, 0, 0), s.Target(new Vector3(9, 0, 0)), "리셋 후 sim 정확 추종");
        }
    }
}
