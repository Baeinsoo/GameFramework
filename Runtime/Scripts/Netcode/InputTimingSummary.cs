namespace GameFramework.Netcode
{
    /// <summary>서버가 측정한 클라 입력 도착 타이밍 한 간격 요약. d = 도착 마진(serverTick − inputTick).</summary>
    public readonly struct InputTimingSummary
    {
        public readonly double AvgD;
        /// <summary>간격 내 가장 큰 d(가장 늦게/빠듯하게 도착한 입력). 클수록 prune 경계에 근접.</summary>
        public readonly int MaxD;
        public readonly int PruneCount;
        /// <summary>유실로 누락된 입력 tick 총수(gap 이벤트 수가 아니라 누락 tick 합).</summary>
        public readonly int SeqGapCount;
        public readonly int SampleCount;

        public InputTimingSummary(double avgD, int maxD, int pruneCount, int seqGapCount, int sampleCount)
        {
            AvgD = avgD;
            MaxD = maxD;
            PruneCount = pruneCount;
            SeqGapCount = seqGapCount;
            SampleCount = sampleCount;
        }

        /// <summary>이번 간격에 측정할 거리가 있었나(아무 입력/실패 없으면 전송 skip).</summary>
        public bool HasActivity => SampleCount > 0 || PruneCount > 0 || SeqGapCount > 0;
    }
}
