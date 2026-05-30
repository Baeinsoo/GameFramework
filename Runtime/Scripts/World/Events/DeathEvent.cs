namespace GameFramework.World
{
    /// <summary>
    /// 사망 사건. 슬라이스 3에선 Application 측 상태 변경 없음 (Health.Current가 이미 0).
    /// Bridge는 슬라이스 3에서 구독자 없어 log-only. 사망 애니메이션·UI 구독은 별도 슬라이스.
    /// </summary>
    public sealed record DeathEvent(
        string victimId,
        string attackerId
    ) : WorldEvent;
}
