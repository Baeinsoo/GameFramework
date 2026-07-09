using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class InterpolationDelayEstimatorTests
    {
        [Test]
        public void NoArrivals_CushionIsBaseline()
        {
            var e = new InterpolationDelayEstimator(sendInterval: 0.033, n: 2, k: 2);
            Assert.AreEqual(2 * 0.033, e.Cushion, 1e-9);
        }

        [Test]
        public void RegularArrivals_NoJitter_CushionStaysBaseline()
        {
            var e = new InterpolationDelayEstimator(0.033, 2, 2);
            double t = 0;
            for (int i = 0; i < 20; i++) { e.RecordArrival(t); t += 0.033; }
            Assert.AreEqual(2 * 0.033, e.Cushion, 1e-6);
        }

        [Test]
        public void JitteryArrivals_CushionGrowsAboveBaseline()
        {
            var e = new InterpolationDelayEstimator(0.033, 2, 2, smoothing: 0.5);
            double t = 0;
            double[] gaps = { 0.033, 0.010, 0.070, 0.005, 0.080, 0.015 };
            foreach (var g in gaps) { t += g; e.RecordArrival(t); }
            Assert.Greater(e.Cushion, 2 * 0.033);
        }

        [Test]
        public void Cushion_ClampedToMax()
        {
            var e = new InterpolationDelayEstimator(0.033, n: 2, k: 100, minCushion: 0, maxCushion: 0.15, smoothing: 1.0);
            e.RecordArrival(0);
            e.RecordArrival(0.5);
            Assert.AreEqual(0.15, e.Cushion, 1e-9);
        }

        [Test]
        public void Cushion_ClampedToMin()
        {
            var e = new InterpolationDelayEstimator(0.033, n: 0, k: 0, minCushion: 0.05, maxCushion: 1.0);
            Assert.AreEqual(0.05, e.Cushion, 1e-9);
        }
    }
}
