using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class SequenceBufferTests
    {
        [Test]
        public void Ctor_NonPositiveCapacity_Throws()
        {
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new SequenceBuffer<int>(0));
            Assert.Throws<System.ArgumentOutOfRangeException>(() => new SequenceBuffer<int>(-1));
        }

        [Test]
        public void Record_ThenTryGet_ReturnsValue()
        {
            var buffer = new SequenceBuffer<int>(4);
            buffer.Record(10, 100);

            Assert.IsTrue(buffer.TryGet(10, out var got));
            Assert.AreEqual(100, got);
        }

        [Test]
        public void TryGet_UnknownTick_ReturnsFalse()
        {
            var buffer = new SequenceBuffer<int>(4);
            buffer.Record(10, 100);

            Assert.IsFalse(buffer.TryGet(9, out _));
        }

        [Test]
        public void Empty_HasNoLatest_AndZeroCount_AndTryGetFalse()
        {
            var buffer = new SequenceBuffer<int>(4);

            Assert.IsFalse(buffer.TryGetLatest(out _));
            Assert.AreEqual(0, buffer.Count);
            Assert.IsFalse(buffer.TryGet(0, out _));
        }

        [Test]
        public void Exceeding_Capacity_EvictsOldestTick()
        {
            var buffer = new SequenceBuffer<int>(4);
            for (long t = 0; t <= 4; t++)   // 용량 4에 5개 기록
            {
                buffer.Record(t, (int)t);
            }

            Assert.IsFalse(buffer.TryGet(0, out _), "가장 오래된 tick 0은 밀려나야 한다");
            Assert.IsTrue(buffer.TryGet(1, out _));
            Assert.IsTrue(buffer.TryGet(4, out var newest));
            Assert.AreEqual(4, newest);
        }

        [Test]
        public void TryGetLatest_ReturnsMostRecentlyRecorded()
        {
            var buffer = new SequenceBuffer<int>(4);
            buffer.Record(7, 70);
            buffer.Record(8, 80);

            Assert.IsTrue(buffer.TryGetLatest(out var latest));
            Assert.AreEqual(80, latest);
        }

        [Test]
        public void Count_CapsAtCapacity()
        {
            var buffer = new SequenceBuffer<int>(4);
            for (long t = 0; t < 10; t++)
            {
                buffer.Record(t, (int)t);
            }

            Assert.AreEqual(4, buffer.Count);
        }

        [Test]
        public void TryGet_UnrecordedTickZeroMappingToDefaultSlot_ReturnsFalse()
        {
            // 병렬 tick 배열의 sentinel이 실제 tick 0과 구분되는지(디폴트 슬롯 함정).
            var buffer = new SequenceBuffer<int>(4);
            buffer.Record(3, 30);   // latest=3, 윈도우가 tick 0을 포함

            Assert.IsFalse(buffer.TryGet(0, out _), "기록 안 된 tick 0은 sentinel과 구분돼 false여야 한다");
        }

        [Test]
        public void ReferenceTypePayload_IsStoredByReference()
        {
            // InputCommand/PredictedAbilityState 같은 참조형 페이로드가 그대로 보존되는지.
            var buffer = new SequenceBuffer<object>(4);
            var payload = new object();
            buffer.Record(5, payload);

            Assert.IsTrue(buffer.TryGet(5, out var got));
            Assert.AreSame(payload, got);
        }

        [Test]
        public void NegativeTick_SlotsSafely()
        {
            // tick 0 근처 음수 조회에도 음수 모듈러가 안전해야 한다.
            var buffer = new SequenceBuffer<int>(4);
            buffer.Record(-2, 42);

            Assert.IsTrue(buffer.TryGet(-2, out var got));
            Assert.AreEqual(42, got);
        }
    }
}
