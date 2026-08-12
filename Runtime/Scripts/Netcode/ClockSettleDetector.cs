using System.Collections.Generic;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 시간에 따라 갱신되는 추정값이 "자리를 잡았는지" 판정한다. 최근 창 안에서 값의 진폭이
    /// 임계 미만이면 안정으로 본다.
    ///
    /// 왜 필요한가: 서버 시간 추정 같은 값은 표본이 쌓이며 수렴한다. 그 값으로 한 번에 확정하는
    /// 일(예: 고정 틱 시뮬의 출발선 긋기)을 덜 익은 시점에 하면 그 순간의 오차가 그대로 굳는다.
    /// <see cref="ClockDilator"/>가 시계를 목표로 수렴시키고 <see cref="LeadController"/>가 앞설 양을
    /// 조정한다면, 이 클래스는 "언제 그 값을 믿어도 되는가"를 답한다.
    ///
    /// 평균이나 기울기가 아니라 진폭(최대−최소)을 보는 이유는 한 표본이 튀어도 창이 지나가면
    /// 정리되기 때문이다. 순수 로직이라 엔진·전송 계층에 의존하지 않는다.
    /// </summary>
    public class ClockSettleDetector
    {
        private readonly double windowSeconds;
        private readonly int minSamples;
        private readonly double amplitudeThreshold;

        private readonly Queue<double> times = new Queue<double>();
        private readonly Queue<double> values = new Queue<double>();

        private double firstTime;
        private bool hasFirst;

        /// <param name="windowSeconds">진폭을 보는 창 길이(초).</param>
        /// <param name="minSamples">
        /// 창 안에 최소 몇 표본이 있어야 판정하나. 호출자가 매 프레임 넣는데 값의 갱신은 더 드문
        /// 경우, 프레임이 정체되면 창에 표본이 두세 개만 남아 진폭이 우연히 작게 나올 수 있다.
        /// </param>
        /// <param name="amplitudeThreshold">창 안 진폭이 이 값 미만이면 안정으로 본다.</param>
        public ClockSettleDetector(double windowSeconds = 0.5, int minSamples = 5, double amplitudeThreshold = 0.005)
        {
            this.windowSeconds = windowSeconds;
            this.minSamples = minSamples;
            this.amplitudeThreshold = amplitudeThreshold;
            Amplitude = double.NaN;
        }

        /// <summary>창 안 표본의 최대−최소. 아직 판정한 적이 없으면 NaN — 0과 구분하려는 것이다.</summary>
        public double Amplitude { get; private set; }

        /// <summary>창 안에 남아 있는 표본 수.</summary>
        public int SampleCount => values.Count;

        /// <summary>마지막 <see cref="Feed"/> 기준으로 안정 조건을 만족하는가.</summary>
        public bool IsSettled { get; private set; }

        /// <summary>
        /// 표본 하나를 넣고 판정을 갱신한다.
        ///
        /// ⚠️ 값의 출처가 아직 비어 있는 동안에는 넣지 말 것. 이 판정기는 "숫자열이 멈췄나"만 보므로,
        /// 아직 아무것도 측정되지 않아 값이 초기값에 고정돼 있으면 완벽히 안정으로 보인다. 그 구분은
        /// 출처만 할 수 있어서 호출자 책임이다(예: 왕복시간이 0이면 아직 응답을 못 받은 것이니 거른다).
        /// </summary>
        /// <param name="now">표본 시각(초). 단조 증가해야 한다.</param>
        /// <param name="value">관찰 중인 추정값.</param>
        public void Feed(double now, double value)
        {
            if (hasFirst == false)
            {
                firstTime = now;
                hasFirst = true;
            }

            times.Enqueue(now);
            values.Enqueue(value);

            // 창 밖으로 나간 표본을 버린다. 방금 넣은 표본은 나이가 0이라 큐가 비지 않는다.
            while (now - times.Peek() > windowSeconds)
            {
                times.Dequeue();
                values.Dequeue();
            }

            if (now - firstTime < windowSeconds || values.Count < minSamples)
            {
                IsSettled = false;
                return;
            }

            double min = double.MaxValue;
            double max = double.MinValue;
            foreach (double v in values)
            {
                if (v < min) min = v;
                if (v > max) max = v;
            }

            Amplitude = max - min;
            IsSettled = Amplitude < amplitudeThreshold;
        }

        /// <summary>다음 매치 등 새 관찰을 시작할 때 부른다.</summary>
        public void Reset()
        {
            times.Clear();
            values.Clear();
            hasFirst = false;
            firstTime = 0;
            Amplitude = double.NaN;
            IsSettled = false;
        }
    }
}
