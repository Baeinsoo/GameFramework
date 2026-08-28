using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class RenderCorrectionSmootherTests
    {
        private const float SmoothTime = 0.1f;
        private const float MinCorrection = 0.025f;
        private const float MaxSmooth = 5f;
        private const float NoSmooth = 8f;

        private static RenderCorrectionSmoother Make()
            => new RenderCorrectionSmoother(SmoothTime, MinCorrection, MaxSmooth, NoSmooth);

        //  보정이 없으면 렌더는 sim을 정확히 따른다(지연 0).
        [Test]
        public void NoCorrection_TargetEqualsSim()
        {
            var s = Make();
            Assert.AreEqual(new Vector3(5, 0, 0), s.Target(new Vector3(5, 0, 0)));
            s.Advance(0.033f);
            Assert.AreEqual(new Vector3(9, 0, 3), s.Target(new Vector3(9, 0, 3)));
        }

        //  보정 직후 첫 렌더는 '있던 자리'에 머문다 — 반대편 오버슈트가 아니다.
        [Test]
        public void OnCorrection_FirstTarget_StaysAtOld()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0), Vector3.Zero, 0f);
            var r = s.Target(new Vector3(0.6f, 0, 0));
            Assert.AreEqual(0f, r.X, 1e-4f);
        }

        //  ★ 이 슬라이스의 핵심. 보정 직후 렌더 속도가 보정 직전 렌더 속도와 이어진다.
        //     지수 감쇠는 여기서 (갭 / 시간상수)만큼 튀어 올랐다.
        //     (deltaTime=0으로 부른다 — 여기서 재는 건 "그 순간의" 수학적 기울기다. 실제
        //     호출 순서에서 그 순간이 정확히 언제 오는지는 아래 ★★ 테스트가 다룬다.)
        [Test]
        public void OnCorrection_RenderVelocity_IsContinuous()
        {
            const float dt = 0.02f;
            var s = Make();

            //  렌더가 +x로 10 m/s로 달리는 상태를 만든다(두 프레임이면 속도를 알 수 있다).
            s.Target(new Vector3(0f, 0, 0));
            s.Advance(dt);
            s.Target(new Vector3(0.2f, 0, 0));
            s.Advance(dt);
            s.Target(new Vector3(0.4f, 0, 0));   // 직전 렌더 위치(세로 속도 0)

            //  세로로 4.788m 튀는 보정(실측 최대치). 권위 속도는 위로 23(=FlapImpulse).
            s.OnCorrection(new Vector3(0.4f, 0, 0), new Vector3(0.4f, 4.788f, 0), new Vector3(0f, 23f, 0), 0f);

            //  보정 직후의 '순간' 기울기를 재야 한다. 간격이 크면 3차식이 그 사이에 휘어서
            //  평균이 잡히므로(0.02초로 재면 33 m/s가 나온다) 아주 짧은 간격으로 잰다.
            const float probeDt = 0.001f;
            var afterPos = s.Target(new Vector3(0.4f, 4.788f, 0));
            s.Advance(probeDt);
            var nextPos = s.Target(new Vector3(0.4f, 4.788f + 23f * probeDt, 0));

            float renderVelY = (nextPos.Y - afterPos.Y) / probeDt;

            //  직전 렌더는 세로로 안 움직였다(0). 연속이면 보정 직후에도 0 근처에서 출발해야 한다.
            //  지수 감쇠였다면 4.788 / 0.1 = 47.9 m/s가 나온다.
            Assert.Less(System.Math.Abs(renderVelY), 5f,
                "보정 직후 세로 렌더 속도가 직전(0)에서 크게 튀면 안 된다 — 지수 감쇠면 47.9가 나온다");
        }

        //  ★★ Finding 2/3 게이트. 실제 호출 순서(Reconcile의 OnCorrection → world.Tick으로
        //  sim이 한 틱 더 진행 → 이 컴포넌트의 Target)를 그대로 흉내낸다. OnCorrection이
        //  elapsed를 0으로 시작하면(옛 버그) 이 첫 스텝이 sim 속도를 따라가 버린다 — 렌더
        //  속도를 따라야 맞다. x축은 sim 속도가 0인데 직전 렌더 속도가 10m/s였던 축이라,
        //  _renderVelocity를 식에서 빼도(Finding 3) 이 값이 달라지므로 테스트 하나로 두
        //  항목을 함께 지킨다.
        [Test]
        public void OnCorrection_FirstStepAfterOneSimTick_FollowsRenderVelocity_NotSimVelocity()
        {
            const float dt = 0.02f;
            var s = Make();

            //  렌더가 +x로 10 m/s로 달리는 상태를 만든다(실제 순서: Target 다음에 Advance).
            s.Target(new Vector3(0f, 0, 0));
            s.Advance(dt);
            s.Target(new Vector3(0.2f, 0, 0));
            s.Advance(dt);
            var beforeCorrection = s.Target(new Vector3(0.4f, 0, 0));   // R(N−1)
            s.Advance(dt);                                              // 그 틱의 사이클을 마저 닫는다

            //  세로로 4.788m 튀는 보정. 권위 속도는 x축 0(서버 쪽 x는 정지), y축 위로 23.
            s.OnCorrection(new Vector3(0.4f, 0, 0), new Vector3(0.4f, 4.788f, 0), new Vector3(0f, 23f, 0), dt);

            //  OnCorrection 뒤 sim이 dt만큼 한 틱 더 진행된 다음에야 Target이 불린다(이
            //  스무더의 Advance는 아직 안 불림 — 그건 Target 다음이다). 이게 실제 호출 순서다.
            var afterCorrection = s.Target(new Vector3(0.4f, 4.788f + 23f * dt, 0));   // R(N)

            float stepX = afterCorrection.X - beforeCorrection.X;

            //  x축 sim 속도는 0, 직전 렌더 속도는 10m/s. elapsed가 여전히 0으로 시작했다면
            //  (이음매가 한 틱 늦게 체결) 이 스텝은 sim 속도(0)를 따라 정확히 0이 된다.
            //  elapsed=dt로 시작해야(u=dt/T의 에르미트 값만큼) 렌더 속도 쪽으로 움직인다.
            //  기대값 0.128은 u=0.2에서의 닫힌 형식 값(coeff·errorSlopeStart.X)이다.
            Assert.AreEqual(0.128f, stepX, 1e-3f,
                "보정 직후 첫 스텝은 u=0이 아니라 u=deltaTime/smoothTime에서 평가돼야 한다");
        }

        //  ★ Finding 1 회귀 방지. 진행 중인 블렌드 도중에 무시할 만큼 작은(예: gap=0인 활공
        //  중 남의 새가 매 틱 부르는 것) 보정이 하나 더 들어와도, 그 진행 중이던 블렌드의
        //  잔여 오차를 지우면 안 된다.
        [Test]
        public void SubMinCorrection_DuringBlend_DoesNotDisturbInProgressBlend()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0), Vector3.Zero, 0f);
            s.Advance(0.02f);
            float midway = s.Target(new Vector3(0.6f, 0, 0)).X;

            //  gap=0인 무관한 보정 — 진행 중인 블렌드를 건드리면 안 된다.
            s.OnCorrection(new Vector3(0.6f, 0, 0), new Vector3(0.6f, 0, 0), Vector3.Zero, 0.02f);

            float afterTiny = s.Target(new Vector3(0.6f, 0, 0)).X;
            Assert.AreEqual(midway, afterTiny, 1e-4f,
                "무시할 만큼 작은 보정이 진행 중이던 블렌드의 잔여 오차를 지우면 안 된다");

            //  그 뒤로도 계속 정상적으로 수렴한다 — 블렌드가 안 끊겼다는 증거.
            s.Advance(0.02f);
            float later = s.Target(new Vector3(0.6f, 0, 0)).X;
            Assert.GreaterOrEqual(later, afterTiny - 1e-4f);
            Assert.LessOrEqual(later, 0.6f + 1e-4f);
        }

        //  정지 상태에서의 보정은 넘지 않고 단조롭게 수렴한다.
        [Test]
        public void StaticCorrection_ConvergesMonotonically_NeverPast()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0), Vector3.Zero, 0f);
            var target = new Vector3(0.6f, 0, 0);
            float prev = s.Target(target).X;
            for (int i = 0; i < 10; i++)
            {
                s.Advance(0.02f);
                float x = s.Target(target).X;
                Assert.GreaterOrEqual(x, prev - 1e-4f, "단조 증가");
                Assert.LessOrEqual(x, 0.6f + 1e-4f, "새 위치를 넘지 않음");
                prev = x;
            }
        }

        //  보간 시간이 지나면 렌더가 sim과 정확히 같아진다.
        [Test]
        public void AfterSmoothTime_ConvergesExactly()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(1f, 0, 0), Vector3.Zero, 0f);
            for (int i = 0; i < 6; i++)   // 6 × 0.02 = 0.12s > SmoothTime
            {
                s.Advance(0.02f);
            }
            Assert.AreEqual(1f, s.Target(new Vector3(1f, 0, 0)).X, 1e-4f);
        }

        //  아주 작은 보정은 스무딩 없이 즉시 채택 — 숨길 튐이 없는데 녹이면 오차만 는다.
        [Test]
        public void SmallCorrection_BelowMin_AdoptsImmediately()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.01f, 0, 0), Vector3.Zero, 0f);
            Assert.AreEqual(new Vector3(0.01f, 0, 0), s.Target(new Vector3(0.01f, 0, 0)));
        }

        //  아주 큰 보정(리스폰 등)은 즉시 스냅 — 녹이면 맵을 가로질러 미끄러진다.
        [Test]
        public void LargeCorrection_AboveNoSmoothDistance_Snaps()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(9f, 0, 0), Vector3.Zero, 0f);   // 9 > 8
            Assert.AreEqual(new Vector3(9f, 0, 0), s.Target(new Vector3(9f, 0, 0)));
        }

        //  목줄: 보간 도중에도 렌더가 sim에서 MaxSmoothDistance 이상 뒤처지지 않는다.
        [Test]
        public void DuringSmoothing_NeverLagsMoreThanMaxSmoothDistance()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            //  7m 보정: NoSmooth(8) 아래라 녹이지만, Max(5)보다 크므로 첫 프레임부터 목줄이 당긴다.
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(7f, 0, 0), Vector3.Zero, 0f);
            var sim = new Vector3(7f, 0, 0);
            for (int i = 0; i < 6; i++)
            {
                float lag = (sim - s.Target(sim)).Length();
                Assert.LessOrEqual(lag, MaxSmooth + 1e-4f, $"i={i}: 목줄보다 더 뒤처지면 안 된다");
                s.Advance(0.02f);
            }
        }

        //  스무딩 시간 0 = 끔(언리얼 NetworkSmoothingMode.Disabled). 내 새가 쓴다.
        [Test]
        public void SmoothTimeZero_Disabled_AlwaysFollowsSim()
        {
            var s = new RenderCorrectionSmoother(0f, MinCorrection, MaxSmooth, NoSmooth);
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(2f, 0, 0), new Vector3(0f, 23f, 0), 0f);
            Assert.AreEqual(new Vector3(2f, 0, 0), s.Target(new Vector3(2f, 0, 0)));
        }

        //  같은 총 시간이면 dt 분할이 달라도 결과가 같다(프레임독립).
        [Test]
        public void Smoothing_FrameRateIndependent()
        {
            var target = new Vector3(1, 0, 0);

            var coarse = Make();
            coarse.Target(new Vector3(0, 0, 0));
            coarse.OnCorrection(new Vector3(0, 0, 0), target, Vector3.Zero, 0f);
            for (int i = 0; i < 5; i++) coarse.Advance(0.01f);    // 0.05s

            var fine = Make();
            fine.Target(new Vector3(0, 0, 0));
            fine.OnCorrection(new Vector3(0, 0, 0), target, Vector3.Zero, 0f);
            for (int i = 0; i < 10; i++) fine.Advance(0.005f);    // 0.05s

            Assert.AreEqual(coarse.Target(target).X, fine.Target(target).X, 1e-4f);
        }

        //  Reset 후에는 sim을 정확히 따른다.
        [Test]
        public void Reset_ClearsOffset()
        {
            var s = Make();
            s.Target(new Vector3(0, 0, 0));
            s.OnCorrection(new Vector3(0, 0, 0), new Vector3(0.6f, 0, 0), Vector3.Zero, 0f);
            s.Target(new Vector3(0.6f, 0, 0));
            s.Reset();
            Assert.AreEqual(new Vector3(9, 0, 0), s.Target(new Vector3(9, 0, 0)));
        }
    }
}
