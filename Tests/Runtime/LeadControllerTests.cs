using NUnit.Framework;
using GameFramework.Netcode;

namespace GameFramework.Tests
{
    public class LeadControllerTests
    {
        private static InputTimingSummary S(double avgD, int maxD, int prune, int seqGap, int samples)
            => new InputTimingSummary(avgD, maxD, prune, seqGap, samples);

        [Test]
        public void Adjust_Failure_IncreasesByBigStep()
        {
            var c = new LeadController(bigStep: 0.010, smallStep: 0.002);
            Assert.AreEqual(0.040, c.Adjust(0.030, S(-3, -3, prune: 1, seqGap: 0, samples: 5)), 1e-9);
        }

        [Test]
        public void Adjust_Tight_IncreasesGraduated()
        {
            var c = new LeadController(tightBand: 1, smallStep: 0.002);
            // maxD=3 → +0.002*(3-1)=+0.004
            Assert.AreEqual(0.034, c.Adjust(0.030, S(0, 3, 0, 0, 5)), 1e-9);
        }

        [Test]
        public void Adjust_Loose_DecreasesBySmallStep()
        {
            var c = new LeadController(looseBand: -1, smallStep: 0.002);
            Assert.AreEqual(0.028, c.Adjust(0.030, S(-6, -4, 0, 0, 5)), 1e-9);
        }

        [Test]
        public void Adjust_DeadZone_NoChange()
        {
            var c = new LeadController(tightBand: 1, looseBand: -1);
            Assert.AreEqual(0.030, c.Adjust(0.030, S(0, 0, 0, 0, 5)), 1e-9);
        }

        [Test]
        public void Adjust_ClampsToMax()
        {
            var c = new LeadController(bigStep: 0.010, maxMargin: 0.035);
            Assert.AreEqual(0.035, c.Adjust(0.030, S(-3, -3, 1, 0, 5)), 1e-9);
        }

        [Test]
        public void Adjust_NoSamplesNoFailure_NoChange()
        {
            var c = new LeadController();
            Assert.AreEqual(0.030, c.Adjust(0.030, S(0, 0, 0, 0, 0)), 1e-9);
        }

        //  기본 밴드의 평형점 = "가장 늦게 온 입력이 1~3틱 이르게". 이 범위 밖으로 나가면 되돌린다.
        //  꼬리가 마감선에 붙는 상태(maxD ≥ 0)를 평형으로 두면 지터 한 번에 곧바로 지각이 된다.

        [TestCase(-1)]
        [TestCase(-2)]
        [TestCase(-3)]
        public void Adjust_DefaultBand_TailOneToThreeTicksEarly_IsDeadZone(int maxD)
        {
            var c = new LeadController();
            Assert.AreEqual(0.030, c.Adjust(0.030, S(-4, maxD, 0, 0, 5)), 1e-9);
        }

        [Test]
        public void Adjust_DefaultBand_TailOnTime_Grows()
        {
            var c = new LeadController();
            // maxD=0 → +0.002*(0-(-1)) = +0.002
            Assert.AreEqual(0.032, c.Adjust(0.030, S(-2, 0, 0, 0, 5)), 1e-9);
        }

        [Test]
        public void Adjust_DefaultBand_TailLate_GrowsMore()
        {
            var c = new LeadController();
            // maxD=+2(2틱 지각) → +0.002*(2-(-1)) = +0.006
            Assert.AreEqual(0.036, c.Adjust(0.030, S(-1.7, 2, 0, 0, 5)), 1e-9);
        }

        [Test]
        public void Adjust_DefaultBand_TailTooEarly_Shrinks()
        {
            var c = new LeadController();
            Assert.AreEqual(0.028, c.Adjust(0.030, S(-6, -4, 0, 0, 5)), 1e-9);
        }
    }
}
