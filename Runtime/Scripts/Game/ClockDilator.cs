namespace GameFramework
{
    /// <summary>
    /// 클럭(시간 값)을 타깃 시간으로 rate time-dilation으로 수렴시킨다. 값을 직접 대입(smoothing)하지 않고
    /// 진행 *속도*를 ±MaxRate로 조정 → 절대 역행하지 않음(결정론/보간 보호). 큰 오차는 1회 snap.
    /// netcode clock sync용(클라가 predictedTime+margin으로 수렴). 순수 함수라 EditMode 테스트 가능.
    /// </summary>
    public class ClockDilator
    {
        private readonly double maxRate;
        private readonly double errorScale;
        private readonly double snapThreshold;

        public ClockDilator(double maxRate = 0.05, double errorScale = 0.1, double snapThreshold = 0.5)
        {
            this.maxRate = maxRate;
            this.errorScale = errorScale;
            this.snapThreshold = snapThreshold;
        }

        /// <summary>current를 target 쪽으로 realDelta 동안 dilation해 advance한 새 값 반환.</summary>
        public double Advance(double current, double target, double realDelta)
        {
            double error = target - current;
            if (System.Math.Abs(error) > snapThreshold)
            {
                return target;
            }
            double dilation = System.Math.Clamp(error / errorScale, -maxRate, maxRate);
            double rate = 1.0 + dilation;
            return current + realDelta * rate;
        }
    }
}
