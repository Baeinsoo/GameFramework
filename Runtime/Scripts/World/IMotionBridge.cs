namespace GameFramework.World
{
    /// <summary>
    /// World 모션을 host(Unity) 물리 바디에 반영하는 포트. 시뮬(world.Tick)이 엔티티별로 호출:
    /// 페이즈 시작 <see cref="SyncTransforms"/> 1회 → 엔티티마다 <see cref="Depenetrate"/> → <see cref="Separate"/> → (KinematicMoveSystem) → <see cref="PushMotion"/>.
    /// 구현은 엔티티의 물리 핸들(공유 PhysicsBody 컴포넌트)로 처리 — per-side 타입을 안 만져 공유 concrete 1개로 둔다
    /// (ICollisionQuery↔UnityCollisionQuery와 같은 "포트 + 공유 concrete" 관용). Entity를 받으므로 IEventSink처럼
    /// World 어셈블리에 둔다.
    /// </summary>
    public interface IMotionBridge
    {
        void SyncTransforms();
        void Depenetrate(Entity entity);
        void Separate(Entity entity);
        void PushMotion(Entity entity);
    }
}
