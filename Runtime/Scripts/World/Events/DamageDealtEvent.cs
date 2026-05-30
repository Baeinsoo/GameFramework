namespace GameFramework.World
{
    /// <summary>
    /// 데미지 적용 결과를 운반한다. attackerId/isCritical/isDodged는 코어 로직이
    /// 추론하지 않고 데이터로 통과시키는 패스 필드 (프레젠테이션이 사용).
    /// remaining/isDead는 적용 후 상태.
    /// </summary>
    public sealed record DamageDealtEvent(
        string targetId,
        string attackerId,
        int    amount,
        bool   isCritical,
        bool   isDodged,
        int    remaining,
        bool   isDead
    ) : WorldEvent;
}
