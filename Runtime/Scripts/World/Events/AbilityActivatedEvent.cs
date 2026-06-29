namespace GameFramework.World
{
    /// <summary>
    /// 어빌리티 발동 연출 신호(cosmetic). 발동 순간(Startup 시작)에 한 번 — 시전자 측 애니/VFX용.
    /// abilityId만 운반하고 어떤 연출인지(cue)는 클라가 마스터데이터로 해석한다(서버는 cue 미보유).
    /// GAS GameplayCue 대응. durable 상태는 싣지 않는다(스냅샷 소관).
    /// </summary>
    public sealed record AbilityActivatedEvent(
        string entityId,
        int    abilityId
    ) : WorldEvent;
}
