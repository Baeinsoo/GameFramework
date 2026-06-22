namespace GameFramework.World
{
    /// <summary>
    /// 데미지 연출 정보를 운반한다(cosmetic). attackerId/isCritical/isDodged는 코어 로직이
    /// 추론하지 않고 데이터로 통과시키는 패스 필드 (프레젠테이션이 사용).
    /// HP는 durable이라 스냅샷으로 가고 이 이벤트엔 싣지 않는다.
    /// </summary>
    public sealed record DamageDealtEvent(
        string targetId,
        string attackerId,
        int    amount,
        bool   isCritical,
        bool   isDodged
    ) : WorldEvent;
}
