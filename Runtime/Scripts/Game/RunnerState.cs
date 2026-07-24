namespace GameFramework
{
    /// <summary>
    /// Runner(호스트) 수명 상태. 행동 없는 "단계 라벨"이라 상태 플래그(enum)로 표현한다.
    /// None은 미초기화 기본값. 전이 규칙·상태별 행동은 없다(그건 매치/앱 FSM의 몫).
    /// </summary>
    public enum RunnerState
    {
        None,
        Initializing,
        Initialized,
        Playing,
        Paused,
        GameOver,
    }
}
