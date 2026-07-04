using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class SnapshotHistoryTests
    {
        private static EntitySnapshot Snap(long tick) =>
            new EntitySnapshot(tick, new Vector3(tick, 0, 0), Quaternion.Identity, Vector3.Zero);

        [Test]
        public void Record_ThenTryGet_ReturnsSnapshot()
        {
            var history = new SnapshotHistory(4);
            history.Record(Snap(10));

            Assert.IsTrue(history.TryGet(10, out var got));
            Assert.AreEqual(10, got.Tick);
            Assert.AreEqual(new Vector3(10, 0, 0), got.Position);
        }

        [Test]
        public void TryGet_UnknownTick_ReturnsFalse()
        {
            var history = new SnapshotHistory(4);
            history.Record(Snap(10));

            Assert.IsFalse(history.TryGet(9, out _));
        }

        [Test]
        public void Empty_HasNoLatest_AndZeroCount()
        {
            var history = new SnapshotHistory(4);

            Assert.IsNull(history.Latest);
            Assert.AreEqual(0, history.Count);
            Assert.IsFalse(history.TryGet(0, out _));
        }

        [Test]
        public void Exceeding_Capacity_EvictsOldestTick()
        {
            var history = new SnapshotHistory(4);
            for (long t = 0; t <= 4; t++)   // 5 records into capacity 4
            {
                history.Record(Snap(t));
            }

            Assert.IsFalse(history.TryGet(0, out _), "가장 오래된 tick 0은 밀려나야 한다");
            Assert.IsTrue(history.TryGet(1, out _));
            Assert.IsTrue(history.TryGet(4, out var newest));
            Assert.AreEqual(4, newest.Tick);
        }

        [Test]
        public void Latest_ReturnsMostRecentlyRecorded()
        {
            var history = new SnapshotHistory(4);
            history.Record(Snap(7));
            history.Record(Snap(8));

            Assert.IsNotNull(history.Latest);
            Assert.AreEqual(8, history.Latest.Value.Tick);
        }

        [Test]
        public void Count_CapsAtCapacity()
        {
            var history = new SnapshotHistory(4);
            for (long t = 0; t < 10; t++)
            {
                history.Record(Snap(t));
            }

            Assert.AreEqual(4, history.Count);
        }

        [Test]
        public void TryGet_UnrecordedTickThatMapsToDefaultSlot_ReturnsFalse()
        {
            // 링 슬롯의 초기 sentinel이 실제 tick 0과 충돌하지 않는지 검증(디폴트 Tick=0 함정).
            var history = new SnapshotHistory(4);
            history.Record(Snap(3));   // latest=3, 윈도우가 tick 0을 포함

            Assert.IsFalse(history.TryGet(0, out _), "기록 안 된 tick 0은 sentinel과 구분돼 false여야 한다");
        }
    }
}
