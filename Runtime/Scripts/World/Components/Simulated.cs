namespace GameFramework.World
{
    /// <summary>
    /// 로컬 시뮬이 이 엔티티의 틱(이동·어빌리티·상태·효과·물리)을 소유한다는 표식(태그 컴포넌트).
    /// host가 사이드 정책대로 부착: 서버=모든 캐릭터 / 클라=예측하는 내 캐릭. LOPWorld.Tick은 이것만 순회.
    /// 안 붙은 엔티티(클라의 남/NPC)는 시뮬 안 함 = 스냅샷 보간 전용.
    /// </summary>
    public class Simulated : Component
    {
    }
}
