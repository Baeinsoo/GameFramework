using GameFramework.Runner;
using NUnit.Framework;

namespace GameFramework.Tests.Runner
{
    public class MatchStartGateTests
    {
        private const long WaitCap = 1500;      // 30초 @50Hz
        private const long Lead = 250;          // 5초 @50Hz — 정착 2초 + 카운트다운 3초

        private static MatchStartGate Gate(int expected = 2)
            => new MatchStartGate(expected, WaitCap, Lead);

        [Test]
        public void 처음엔_대기_상태이고_출발틱이_없다()
        {
            var gate = Gate();
            gate.Tick(1000);

            Assert.AreEqual(MatchPhase.WaitingForPlayers, gate.Phase);
            Assert.AreEqual(long.MaxValue, gate.StartTick);
        }

        [Test]
        public void 전원_준비하면_그_틱에_카운트다운이_시작된다()
        {
            var gate = Gate();
            gate.Tick(1000);

            gate.MarkReady("a");
            gate.MarkReady("b");
            gate.Tick(1001);

            Assert.AreEqual(MatchPhase.Countdown, gate.Phase);
            Assert.AreEqual(1001 + Lead, gate.StartTick);
        }

        [Test]
        public void 일부만_준비했고_상한_전이면_계속_기다린다()
        {
            var gate = Gate();
            gate.Tick(1000);

            gate.MarkReady("a");
            gate.Tick(1000 + WaitCap - 1);

            Assert.AreEqual(MatchPhase.WaitingForPlayers, gate.Phase);
            Assert.AreEqual(1, gate.ReadyCount);
        }

        [Test]
        public void 상한이_지나면_안_온_사람을_두고_출발한다()
        {
            var gate = Gate();
            gate.Tick(1000);

            gate.MarkReady("a");
            gate.Tick(1000 + WaitCap);

            Assert.AreEqual(MatchPhase.Countdown, gate.Phase);
            Assert.AreEqual(1000 + WaitCap + Lead, gate.StartTick);
        }

        [Test]
        public void 같은_사람이_여러_번_보내도_한_명으로_센다()
        {
            var gate = Gate();
            gate.Tick(1000);

            gate.MarkReady("a");
            gate.MarkReady("a");
            gate.MarkReady("a");
            gate.Tick(1001);

            Assert.AreEqual(1, gate.ReadyCount);
            Assert.AreEqual(MatchPhase.WaitingForPlayers, gate.Phase);
        }

        [Test]
        public void 카운트다운_중에_나머지가_준비해도_출발틱은_안_밀린다()
        {
            var gate = Gate();
            gate.Tick(1000);
            gate.MarkReady("a");
            gate.Tick(1000 + WaitCap);          // 상한 만료로 카운트다운 진입
            long announced = gate.StartTick;

            gate.MarkReady("b");
            gate.Tick(1000 + WaitCap + 10);

            Assert.AreEqual(announced, gate.StartTick);
            Assert.AreEqual(MatchPhase.Countdown, gate.Phase);
        }

        [Test]
        public void 출발틱_직전까지는_아직_카운트다운이다()
        {
            var gate = Gate();
            gate.Tick(1000);
            gate.MarkReady("a");
            gate.MarkReady("b");
            gate.Tick(1001);

            gate.Tick(gate.StartTick - 1);

            Assert.AreEqual(MatchPhase.Countdown, gate.Phase);
        }

        [Test]
        public void 출발틱에_진행으로_바뀐다()
        {
            var gate = Gate();
            gate.Tick(1000);
            gate.MarkReady("a");
            gate.MarkReady("b");
            gate.Tick(1001);

            gate.Tick(gate.StartTick);

            Assert.AreEqual(MatchPhase.InProgress, gate.Phase);
        }

        [Test]
        public void 진행_후에_온_준비는_아무것도_바꾸지_않는다()
        {
            var gate = Gate(expected: 3);
            gate.Tick(1000);
            gate.MarkReady("a");
            gate.Tick(1000 + WaitCap);
            long announced = gate.StartTick;
            gate.Tick(announced);

            gate.MarkReady("b");
            gate.Tick(announced + 1);

            Assert.AreEqual(MatchPhase.InProgress, gate.Phase);
            Assert.AreEqual(announced, gate.StartTick);
            Assert.AreEqual(1, gate.ReadyCount);
        }

        [Test]
        public void 아무도_없는_방은_기다리지_않고_바로_카운트다운한다()
        {
            var gate = Gate(expected: 0);
            gate.Tick(1000);

            Assert.AreEqual(MatchPhase.Countdown, gate.Phase);
            Assert.AreEqual(1000 + Lead, gate.StartTick);
        }

        [Test]
        public void 끝난_매치는_더_이상_전이하지_않는다()
        {
            var gate = Gate();
            gate.Tick(1000);
            gate.MarkReady("a");
            gate.MarkReady("b");
            gate.Tick(1001);
            gate.Tick(gate.StartTick);

            gate.Finish();
            gate.Tick(gate.StartTick + 100);

            Assert.AreEqual(MatchPhase.Finished, gate.Phase);
        }
    }
}
