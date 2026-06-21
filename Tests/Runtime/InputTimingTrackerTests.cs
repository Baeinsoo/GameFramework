using NUnit.Framework;

namespace GameFramework.Tests
{
    public class InputTimingTrackerTests
    {
        [Test]
        public void Summarize_NoSamples_ZeroAndNoActivity()
        {
            var t = new InputTimingTracker();
            var s = t.Summarize();
            Assert.AreEqual(0.0, s.AvgD, 1e-9);
            Assert.AreEqual(0, s.MaxD);
            Assert.AreEqual(0, s.SampleCount);
            Assert.IsFalse(s.HasActivity);
        }

        [Test]
        public void Summarize_Arrivals_AveragesAndKeepsTightestMax()
        {
            var t = new InputTimingTracker();
            t.RecordArrival(-5);
            t.RecordArrival(-3);
            t.RecordArrival(-1);
            var s = t.Summarize();
            Assert.AreEqual(-3.0, s.AvgD, 1e-9);
            Assert.AreEqual(-1, s.MaxD);
            Assert.AreEqual(3, s.SampleCount);
            Assert.IsTrue(s.HasActivity);
        }

        [Test]
        public void Summarize_CountsPruneAndSeqGap_ActivityEvenWithoutSamples()
        {
            var t = new InputTimingTracker();
            t.RecordPrune();
            t.RecordSeqGap(2);
            var s = t.Summarize();
            Assert.AreEqual(1, s.PruneCount);
            Assert.AreEqual(2, s.SeqGapCount);
            Assert.AreEqual(0, s.SampleCount);
            Assert.IsTrue(s.HasActivity);
        }

        [Test]
        public void Summarize_ResetsAccumulators()
        {
            var t = new InputTimingTracker();
            t.RecordArrival(-2);
            t.RecordPrune();
            t.RecordSeqGap(1);
            t.Summarize();
            var s = t.Summarize();
            Assert.AreEqual(0, s.SampleCount);
            Assert.AreEqual(0, s.PruneCount);
            Assert.AreEqual(0, s.SeqGapCount);
            Assert.IsFalse(s.HasActivity);
        }
    }
}
