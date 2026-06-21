namespace GameFramework
{
    /// <summary>
    /// 서버측 per-client 입력 도착 타이밍 누적기. 한 피드백 간격 동안 도착 마진 d 샘플과
    /// prune/seq-gap 실패를 모아 Summarize()로 요약하고 비운다. 순수 C#이라 EditMode 테스트 가능.
    /// </summary>
    public class InputTimingTracker
    {
        private double _dSum;
        private int _maxD = int.MinValue;
        private int _sampleCount;
        private int _pruneCount;
        private int _seqGapCount;

        /// <summary>입력이 처음 버퍼에 들어올 때의 도착 마진 d = serverTick − inputTick.</summary>
        public void RecordArrival(int d)
        {
            _dSum += d;
            if (d > _maxD)
            {
                _maxD = d;
            }
            _sampleCount++;
        }

        /// <summary>처리 시점이 지나 폐기된 입력(너무 늦음) 1건.</summary>
        public void RecordPrune() => _pruneCount++;

        /// <summary>소비 seq 불연속으로 감지된 유실 입력 gap건.</summary>
        public void RecordSeqGap(int gap)
        {
            if (gap > 0)
            {
                _seqGapCount += gap;
            }
        }

        /// <summary>간격 요약을 산출하고 누적기를 비운다.</summary>
        public InputTimingSummary Summarize()
        {
            double avgD = _sampleCount > 0 ? _dSum / _sampleCount : 0.0;
            int maxD = _sampleCount > 0 ? _maxD : 0;
            var summary = new InputTimingSummary(avgD, maxD, _pruneCount, _seqGapCount, _sampleCount);

            _dSum = 0;
            _maxD = int.MinValue;
            _sampleCount = 0;
            _pruneCount = 0;
            _seqGapCount = 0;

            return summary;
        }
    }
}
