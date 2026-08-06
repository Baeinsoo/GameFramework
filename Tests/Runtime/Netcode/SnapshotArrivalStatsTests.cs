using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class SnapshotArrivalStatsTests
    {
        [Test]
        public void NoArrivals_LatestTickIsMinusOne()
        {
            var s = new SnapshotArrivalStats();
            Assert.AreEqual(-1, s.LatestTick);
            Assert.AreEqual(0, s.SampleCount);
            Assert.AreEqual(0.0, s.AverageInterval, 1e-9);
            Assert.AreEqual(0.0, s.MaxInterval, 1e-9);
        }

        [Test]
        public void FirstArrival_SetsTick_ButHasNoIntervalYet()
        {
            var s = new SnapshotArrivalStats();
            s.Record(10, 1.0);
            Assert.AreEqual(10, s.LatestTick);
            Assert.AreEqual(0, s.SampleCount);
        }

        [Test]
        public void SecondArrival_RecordsInterval()
        {
            var s = new SnapshotArrivalStats();
            s.Record(10, 1.0);
            s.Record(11, 1.05);
            Assert.AreEqual(11, s.LatestTick);
            Assert.AreEqual(1, s.SampleCount);
            Assert.AreEqual(0.05, s.AverageInterval, 1e-9);
            Assert.AreEqual(0.05, s.MaxInterval, 1e-9);
        }

        [Test]
        public void StaleOrDuplicateTick_IsIgnored()
        {
            var s = new SnapshotArrivalStats();
            s.Record(10, 1.0);
            s.Record(11, 1.05);
            s.Record(11, 1.06);   // 같은 틱이 청킹돼 또 온 경우
            s.Record(9, 1.07);    // 순서가 뒤집혀 온 경우
            Assert.AreEqual(11, s.LatestTick);
            Assert.AreEqual(1, s.SampleCount);
            Assert.AreEqual(0.05, s.MaxInterval, 1e-9);
        }

        [Test]
        public void MaxInterval_KeepsLargest()
        {
            var s = new SnapshotArrivalStats();
            s.Record(1, 0.0);
            s.Record(2, 0.02);
            s.Record(3, 0.20);
            s.Record(4, 0.22);
            Assert.AreEqual(0.18, s.MaxInterval, 1e-9);
        }

        [Test]
        public void Average_IsMeanOfIntervals()
        {
            var s = new SnapshotArrivalStats();
            s.Record(1, 0.0);
            s.Record(2, 0.10);
            s.Record(3, 0.30);
            Assert.AreEqual(3, s.LatestTick);
            Assert.AreEqual(2, s.SampleCount);
            Assert.AreEqual(0.15, s.AverageInterval, 1e-9);
        }

        [Test]
        public void Reset_ClearsEverything()
        {
            var s = new SnapshotArrivalStats();
            s.Record(1, 0.0);
            s.Record(2, 0.30);
            s.Reset();
            Assert.AreEqual(-1, s.LatestTick);
            Assert.AreEqual(0, s.SampleCount);
            Assert.AreEqual(0.0, s.AverageInterval, 1e-9);
            Assert.AreEqual(0.0, s.MaxInterval, 1e-9);
        }

        [Test]
        public void AfterReset_NextArrivalStartsFresh()
        {
            var s = new SnapshotArrivalStats();
            s.Record(1, 0.0);
            s.Record(2, 0.30);
            s.Reset();
            s.Record(3, 5.0);   // 리셋 직후 첫 도착 — 리셋 전 시각과의 간격을 만들면 안 된다
            Assert.AreEqual(0, s.SampleCount);
            Assert.AreEqual(0.0, s.MaxInterval, 1e-9);
        }
    }
}
