namespace GameFramework.Netcode
{
    /// <summary>
    /// 스냅 배치 도착 간격의 지터로 적응형 보간 쿠션을 산출. 순수 — EditMode 테스트.
    /// 쿠션 = clamp(n·sendInterval + k·jitter, min, max). jitter = |도착간격 − sendInterval|의 EWMA.
    /// 핑(레이턴시)은 여기 안 들어감 — 도착 "간격"만 본다.
    /// </summary>
    public class InterpolationDelayEstimator
    {
        private readonly double sendInterval;
        private readonly double n;
        private readonly double k;
        private readonly double minCushion;
        private readonly double maxCushion;
        private readonly double smoothing;

        private double lastArrival;
        private bool hasLast;
        private double jitter;

        public InterpolationDelayEstimator(double sendInterval, double n = 2, double k = 2,
            double minCushion = 0, double maxCushion = double.MaxValue, double smoothing = 0.1)
        {
            this.sendInterval = sendInterval;
            this.n = n;
            this.k = k;
            this.minCushion = minCushion;
            this.maxCushion = maxCushion;
            this.smoothing = smoothing;
        }

        public void RecordArrival(double arrivalTime)
        {
            if (hasLast)
            {
                double interval = arrivalTime - lastArrival;
                double deviation = System.Math.Abs(interval - sendInterval);
                jitter += smoothing * (deviation - jitter);
            }
            lastArrival = arrivalTime;
            hasLast = true;
        }

        public double Cushion =>
            System.Math.Clamp(n * sendInterval + k * jitter, minCushion, maxCushion);
    }
}
