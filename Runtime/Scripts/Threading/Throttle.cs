using System;

namespace GameFramework.Threading
{
    /// <summary>정해진 간격 안에 다시 물으면 막는다. 실패가 반복될 때 같은 요청을 계속
    /// 내보내는 것을 방지한다.</summary>
    /// <remarks>시각을 인자로 받는다 — 테스트가 시간을 앞당길 수 있어야 하고, 게임 클럭과
    /// 무관하게 실제 경과 시간으로 판단해야 하기 때문.</remarks>
    public class Throttle
    {
        private readonly TimeSpan interval;

        private DateTimeOffset? lastAcquiredAt;

        public Throttle(TimeSpan interval)
        {
            this.interval = interval;
        }

        public bool TryAcquire(DateTimeOffset now)
        {
            if (lastAcquiredAt.HasValue && now - lastAcquiredAt.Value < interval)
            {
                return false;
            }

            //  통과한 호출만 시각을 갱신한다 — 막힌 호출까지 밀면 창이 무한히 연장된다.
            lastAcquiredAt = now;
            return true;
        }
    }
}
