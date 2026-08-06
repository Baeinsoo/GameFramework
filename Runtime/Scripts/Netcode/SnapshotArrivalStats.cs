using System.Collections.Generic;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 스냅 배치가 얼마나 고르게 도착하는지 재는 통계. 순수 — EditMode 테스트.
    /// 서버가 자기 틱을 못 지키면 도착 간격이 흔들리고, 최신 tick이 벽시계 기준 추정보다 뒤처진다.
    /// 그 두 가지를 클라에서 보기 위한 재료다.
    /// </summary>
    public class SnapshotArrivalStats
    {
        // 평균 창(샘플 수). 50Hz 기준 약 1.2초 — 조건이 바뀌면 그만큼 안에 값이 따라온다.
        private const int WindowSize = 60;

        private readonly Queue<double> window = new Queue<double>(WindowSize);
        private double sum;
        private double lastArrival;
        private bool hasLast;

        /// <summary>가장 최근에 받은 서버 tick. 아직 하나도 못 받았으면 -1.</summary>
        public long LatestTick { get; private set; } = -1;

        public double AverageInterval { get; private set; }

        public double MaxInterval { get; private set; }

        public int SampleCount => window.Count;

        /// <summary>
        /// 호출자는 틱당 한 번만 부른다. 같은 틱이 여러 메시지로 쪼개져 와도 첫 것만 — 간격이
        /// 0에 가깝게 찍혀 통계가 망가지는 걸 막는다. 순서가 뒤집혀 온 오래된 틱도 무시한다.
        /// </summary>
        public void Record(long serverTick, double arrivalTime)
        {
            if (serverTick <= LatestTick)
            {
                return;
            }
            LatestTick = serverTick;

            if (hasLast)
            {
                double interval = arrivalTime - lastArrival;
                if (interval > MaxInterval)
                {
                    MaxInterval = interval;
                }
                window.Enqueue(interval);
                sum += interval;
                if (window.Count > WindowSize)
                {
                    sum -= window.Dequeue();
                }
                AverageInterval = sum / window.Count;
            }
            lastArrival = arrivalTime;
            hasLast = true;
        }

        /// <summary>실험 조건을 바꿀 때 부른다. 이전 조건의 최대값이 다음 조건에 섞이지 않게.</summary>
        public void Reset()
        {
            window.Clear();
            sum = 0;
            lastArrival = 0;
            hasLast = false;
            LatestTick = -1;
            AverageInterval = 0;
            MaxInterval = 0;
        }
    }
}
