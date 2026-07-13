namespace GameFramework
{
    /// <summary>
    /// 키네마틱 이동의 host(Unity) 결합을 사이드 무관하게 뺀 포트. 시뮬(world.Tick)이 엔티티별로 호출:
    /// 페이즈 시작 <see cref="SyncTransforms"/> 1회 → 엔티티마다 <see cref="Depenetrate"/> → (KinematicMoveSystem) → <see cref="PushMotion"/>.
    /// 구체는 사이드별(LOPEntity/PhysicsComponent 래핑) — ICollisionQuery/IOverlapQuery와 같은 포트 관용.
    /// </summary>
    public interface IMotionBridge
    {
        void SyncTransforms();
        void Depenetrate(string entityId);
        void PushMotion(string entityId);
    }
}
