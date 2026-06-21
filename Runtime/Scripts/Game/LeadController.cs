namespace GameFramework
{
    /// <summary>
    /// 서버 입력 타이밍 피드백(도착 마진 d + 실패)으로 클라 lead(AheadMargin, 초)를 조정하는 정책.
    /// 오버워치식 dead-zone + 계단식 밴드 + 비대칭(실패 시 빠르게 늘림 / 여유 과다 시 천천히 줄임).
    /// 증분이라 피드백 1건당 1회 호출(매 프레임 아님). 순수 함수라 EditMode 테스트 가능.
    /// 임계값은 Stage 1 관측 데이터로 튜닝(아래 기본은 출발점).
    /// </summary>
    public class LeadController
    {
        private readonly int tightBand;
        private readonly int looseBand;
        private readonly double bigStep;
        private readonly double smallStep;
        private readonly double minMargin;
        private readonly double maxMargin;

        public LeadController(int tightBand = 1, int looseBand = -1,
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
