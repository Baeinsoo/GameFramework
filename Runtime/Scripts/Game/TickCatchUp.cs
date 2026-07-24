namespace GameFramework
{
    /// <summary>
    /// 프레임당 틱 캐치업 상한 계산(순수). 한 프레임에 처리할 틱을 상한으로 잘라,
    /// 히칭·큰 시간 점프 때 무한 틱을 돌다 멈추는 spiral of death를 막는다.
    /// </summary>
    public static class TickCatchUp
    {
        /// <summary>
        /// 이번 프레임에 처리할 틱의 (포함) 상한. 처리할 게 없으면 tick-1을 반환해
        /// 호출부의 while (tick &lt;= 반환값) 루프가 0회 돌게 한다.
        /// </summary>
        public static long ClampTarget(long tick, long processibleTick, int maxTicksPerFrame)
        {
            if (processibleTick < tick)
            {
                return tick - 1;
            }

            int cap = maxTicksPerFrame < 1 ? 1 : maxTicksPerFrame;
            long frameEnd = tick + cap - 1;
            return frameEnd < processibleTick ? frameEnd : processibleTick;
        }
    }
}
