namespace GameFramework.Runner
{
    /// <summary>
    /// 매치 진행 단계. 언리얼 AGameMode::MatchState에 대응한다.
    /// 서버 안에서만 쓴다 — 와이어로는 "몇 틱에 출발"만 나간다(현재 상태를 보내면 앞서 달리는
    /// 클라에게는 이미 지난 정보가 되기 때문).
    /// </summary>
    public enum MatchPhase
    {
        WaitingForPlayers,
        Countdown,
        InProgress,
        Finished,
    }
}
