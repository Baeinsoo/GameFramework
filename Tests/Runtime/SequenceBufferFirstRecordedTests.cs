using NUnit.Framework;
using GameFramework.Netcode;

namespace GameFramework.Tests
{
    /// <summary>
    /// 조회 실패의 두 원인을 가르기 위한 <see cref="SequenceBuffer{T}.FirstRecordedTick"/> 계약.
    ///
    /// 넷코드에서 "그 틱의 기록이 없다"는 두 가지 뜻이다 — ① 내가 아직 살지 않은 틱(매치 참가 전)
    /// ② 살았지만 링 밖으로 밀려난 틱(내가 크게 뒤처짐). ①은 서버 상태가 내 예측을 반증하지 못하므로
    /// 손대면 안 되고, ②는 따라잡아야 한다. 둘을 가르는 기준이 "내가 처음 기록한 틱"이다.
    /// </summary>
    public class SequenceBufferFirstRecordedTests
    {
        [Test]
        public void FirstRecordedTick_BeforeAnyRecord_IsNull()
        {
            var buffer = new SequenceBuffer<int>(8);
            Assert.IsNull(buffer.FirstRecordedTick);
        }

        [Test]
        public void FirstRecordedTick_AfterFirstRecord_IsThatTick()
        {
            var buffer = new SequenceBuffer<int>(8);
            buffer.Record(440, 1);
            Assert.AreEqual(440, buffer.FirstRecordedTick);
        }

        [Test]
        public void FirstRecordedTick_DoesNotMoveAsRingWraps()
        {
            var buffer = new SequenceBuffer<int>(4);
            for (long t = 440; t < 460; t++)
            {
                buffer.Record(t, (int)t);
            }
            // 링이 여러 바퀴 돌아 440은 이미 밀려났지만, "언제 시작했나"는 그대로여야 한다.
            Assert.AreEqual(440, buffer.FirstRecordedTick);
            Assert.IsFalse(buffer.TryGet(440, out _));
        }

        [Test]
        public void PreJoinTick_IsDistinguishableFromEvictedTick()
        {
            var buffer = new SequenceBuffer<int>(4);
            for (long t = 440; t < 450; t++)
            {
                buffer.Record(t, (int)t);
            }

            // ① 참가 전 틱 — 조회도 실패하고 첫 기록보다도 과거다.
            Assert.IsFalse(buffer.TryGet(432, out _));
            Assert.Less(432, buffer.FirstRecordedTick.Value);

            // ② 밀려난 틱 — 조회는 실패하지만 첫 기록 이후다(내가 살았던 틱).
            Assert.IsFalse(buffer.TryGet(441, out _));
            Assert.GreaterOrEqual(441, buffer.FirstRecordedTick.Value);
        }

        [Test]
        public void FirstRecordedTick_RecordsOutOfOrder_KeepsTheEarliestSeen()
        {
            var buffer = new SequenceBuffer<int>(8);
            buffer.Record(450, 1);
            buffer.Record(445, 2);
            // 재생 중 과거 틱을 다시 기록하는 경로가 있으므로, 더 이른 틱을 보면 낮춘다.
            Assert.AreEqual(445, buffer.FirstRecordedTick);
        }
    }
}
