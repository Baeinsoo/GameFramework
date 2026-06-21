namespace GameFramework
{
    /// <summary>서버가 측정한 클라 입력 도착 타이밍 한 간격 요약. d = 도착 마진(serverTick − inputTick).</summary>
    public readonly struct InputTimingSummary
    {
        public readonly double AvgD;
        public readonly int MaxD;
        public readonly int PruneCount;
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
