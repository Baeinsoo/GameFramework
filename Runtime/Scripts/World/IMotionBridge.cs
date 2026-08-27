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

        /// <summary>
        /// 지오메트리에 파묻힌 엔티티를 밖으로 밀어내고, <b>실제로 민 벡터</b>를 돌려준다(안 밀었으면 0).
        /// 부르는 쪽이 이 값을 쓰는 이유: 캡슐이 이미 콜라이더 <i>안</i>에서 시작하면 sweep은 히트를
        /// 못 낸다(시작 겹침은 무시된다). 그래서 "벽에 닿았으니 속도를 지운다"가 실행되지 않아,
        /// 막혀 있는데 중력만 계속 쌓이고 밀어내기와 줄다리기를 하게 된다. 민 방향을 알면 그 방향으로
        /// 파고들던 속도를 지워 줄다리기를 끊을 수 있다(표준 키네마틱 컨트롤러가 하는 일).
        /// </summary>
        System.Numerics.Vector3 Depenetrate(Entity entity);
        void Separate(Entity entity);
        void PushMotion(Entity entity);
    }
}
