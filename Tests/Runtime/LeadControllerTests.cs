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
    }
}
