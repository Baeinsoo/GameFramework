namespace GameFramework.Netcode
{
    /// <summary>
    /// 서버 입력 타이밍 피드백(도착 마진 d + 실패)으로 클라 lead(AheadMargin, 초)를 조정하는 정책.
    /// 오버워치식 dead-zone + 계단식 밴드 + 비대칭(실패 시 빠르게 늘림 / 여유 과다 시 천천히 줄임).
    /// 증분이라 피드백 1건당 1회 호출(매 프레임 아님). 순수 함수라 EditMode 테스트 가능.
    ///
    /// <para>기본 밴드의 평형점은 <b>가장 늦게 온 입력이 1~3틱 이르게</b> 도착하는 상태다
    /// (maxD ∈ [-3, -1]). 꼬리를 마감선에 붙이면(옛 기본값 [-1, +1]) 지터가 한 번만 튀어도
    /// 곧바로 지각·폐기가 되고, 유실 복구 사본(원본보다 1~2틱 늦게 온다)은 아예 못 쓴다.
    /// Unity Netcode for Entities의 <c>TargetCommandSlack</c>(기본 2틱)과 같은 자리.</para>
    /// </summary>
    public class LeadController
    {
        private readonly int tightBand;
        private readonly int looseBand;
        private readonly double bigStep;
        private readonly double smallStep;
        private readonly double minMargin;
        private readonly double maxMargin;

        public LeadController(int tightBand = -1, int looseBand = -3,
            double bigStep = 0.010, double smallStep = 0.002,
            double minMargin = 0.0, double maxMargin = 0.100)
        {
            this.tightBand = tightBand;
            this.looseBand = looseBand;
            this.bigStep = bigStep;
            this.smallStep = smallStep;
            this.minMargin = minMargin;
            this.maxMargin = maxMargin;
        }

        /// <summary>현재 margin(초)과 이번 간격 요약으로 새 margin(초) 반환.</summary>
        public double Adjust(double currentMargin, InputTimingSummary summary)
        {
            double margin = currentMargin;

            if (summary.PruneCount > 0 || summary.SeqGapCount > 0)
            {
                margin += bigStep;                                  // 실패 = 비상, 빠르게 쿠션 추가
            }
            else if (summary.SampleCount > 0 && summary.MaxD > tightBand)
            {
                margin += smallStep * (summary.MaxD - tightBand);   // 빠듯 = 계단식 증가
            }
            else if (summary.SampleCount > 0 && summary.MaxD < looseBand)
            {
                margin -= smallStep;                                // 여유 = 천천히 감소
            }
            // dead-zone: looseBand ≤ maxD ≤ tightBand → 변화 없음

            return System.Math.Clamp(margin, minMargin, maxMargin);
        }
    }
}
