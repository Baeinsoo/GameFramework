using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class PredictionErrorSmootherTests
    {
        private const float SmoothTime = 0.1f;
        private const float MinCorrection = 0.025f;
        private const float TeleportDistance = 10f;

        private static PredictionErrorSmoother Make()
            => new PredictionErrorSmoother(SmoothTime, MinCorrection, TeleportDistance);

        private static PredictionErrorSmoother Corrected(Vector3 oldSim, Vector3 newSim)
        {
            var s = Make();
            s.Target(oldSim);
            s.OnCorrection(oldSim, newSim, Vector3.Zero, 0f);
            return s;
        }

        [Test]
        public void 보정이_없으면_시뮬_자리를_그대로_그린다()
        {
            var s = Make();
            Assert.AreEqual(new Vector3(5, 0, 0), s.Target(new Vector3(5, 0, 0)));
            s.Advance(0.05f);
            Assert.AreEqual(new Vector3(9, 0, 3), s.Target(new Vector3(9, 0, 3)));
        }

        //  소스 엔진 cl_smooth: 보정 순간엔 있던 자리에 있고, 남은 오차를 시간에 비례해 일정하게 줄인다.
        [Test]
        public void 보정_직후는_있던_자리_절반_시간엔_절반_다_지나면_시뮬_자리()
        {
            var s = Corrected(Vector3.Zero, new Vector3(4f, 0, 0));
            Assert.AreEqual(0f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f);

            s.Advance(0.05f);
            Assert.AreEqual(2f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f, "0.05초 뒤엔 4m 오차의 절반이 남는다");

            s.Advance(0.05f);
            Assert.AreEqual(4f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f);
        }

        //  남의 몸용 목줄(5m)과 즉시 채택(8m) 문턱이 내 몸 넉백 보정을 자르면 안 된다.
        [Test]
        public void 판_후반_큰_넉백_9m도_첫_프레임에_건너뛰지_않는다()
        {
            var s = Corrected(Vector3.Zero, new Vector3(9f, 0, 0));
            Assert.AreEqual(0f, s.Target(new Vector3(9f, 0, 0)).X, 1e-4f);
        }

        [Test]
        public void 텔레포트_거리를_넘으면_녹이지_않고_바로_간다()
        {
            var s = Corrected(Vector3.Zero, new Vector3(10.5f, 0, 0));
            Assert.AreEqual(10.5f, s.Target(new Vector3(10.5f, 0, 0)).X, 1e-4f);
        }

        [Test]
        public void 아주_작은_보정은_바로_맞춘다()
        {
            var s = Corrected(Vector3.Zero, new Vector3(0.02f, 0, 0));
            Assert.AreEqual(0.02f, s.Target(new Vector3(0.02f, 0, 0)).X, 1e-6f);
        }

        //  녹이는 도중 새 보정: 남은 오차에 새 오차를 더해 처음부터 다시 줄인다 — 시뮬을 지나치지 않는다.
        [Test]
        public void 녹이는_도중_새_보정은_남은_오차에_더하고_지나치지_않는다()
        {
            var s = Corrected(Vector3.Zero, new Vector3(4f, 0, 0));
            s.Advance(0.05f);
            Assert.AreEqual(2f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f);

            //  시뮬이 4 → 7로 다시 옮겨졌다. 화면은 지금 그리던 2에서 출발해야 한다.
            s.OnCorrection(new Vector3(4f, 0, 0), new Vector3(7f, 0, 0), new Vector3(30f, 0, 0), 0f);
            Assert.AreEqual(2f, s.Target(new Vector3(7f, 0, 0)).X, 1e-4f);

            for (int i = 0; i < 10; i++)
            {
                s.Advance(0.01f);
                float x = s.Target(new Vector3(7f, 0, 0)).X;
                Assert.LessOrEqual(x, 7f + 1e-4f, "시뮬 자리를 넘어갔다 돌아오면 안 된다");
            }
            Assert.AreEqual(7f, s.Target(new Vector3(7f, 0, 0)).X, 1e-4f);
        }

        //  호출 순서상 보정 뒤 첫 Target은 시뮬이 한 틱 더 간 뒤에 온다 — 그 한 틱을 미리 센다.
        [Test]
        public void 보정에_넘긴_틱_시간만큼_이미_줄어_있다()
        {
            var s = Make();
            s.Target(Vector3.Zero);
            s.OnCorrection(Vector3.Zero, new Vector3(4f, 0, 0), Vector3.Zero, 0.05f);
            Assert.AreEqual(2f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f);
        }

        [Test]
        public void 한_번도_안_그렸으면_녹이지_않는다()
        {
            var s = Make();
            s.OnCorrection(Vector3.Zero, new Vector3(4f, 0, 0), Vector3.Zero, 0f);
            Assert.AreEqual(4f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f);
        }

        [Test]
        public void 텔레포트는_남은_오차를_버린다()
        {
            var s = Corrected(Vector3.Zero, new Vector3(4f, 0, 0));
            s.OnTeleport();
            Assert.AreEqual(4f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f);
        }

        [Test]
        public void 녹이는_시간이_0이면_늘_바로_맞춘다()
        {
            var s = new PredictionErrorSmoother(0f, MinCorrection, TeleportDistance);
            s.Target(Vector3.Zero);
            s.OnCorrection(Vector3.Zero, new Vector3(4f, 0, 0), Vector3.Zero, 0f);
            Assert.AreEqual(4f, s.Target(new Vector3(4f, 0, 0)).X, 1e-4f);
        }
    }
}
