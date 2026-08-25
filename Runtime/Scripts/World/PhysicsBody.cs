using System.Numerics;

namespace GameFramework.World
{
    /// <summary>
    /// 엔티티의 물리 몸(엔진 리지드바디/콜라이더)에 대한 순수 포트(헥사고날 ports-and-adapters).
    /// 코어·시뮬은 이 추상만 알고, Unity 어댑터(예: UnityPhysicsBody)가 실제 엔진 조작을 구현한다.
    /// 값은 엔진 비의존 System.Numerics로 주고받아 World 어셈블리의 noEngineReferences를 지킨다.
    /// </summary>
    public abstract class PhysicsBody : Component
    {
        /// <summary>키네마틱(우리가 위치를 직접 미는) 바디인가.</summary>
        public abstract bool IsKinematic { get; }

        public abstract void SetPosition(Vector3 position);
        public abstract void SetRotation(Quaternion rotation);
        public abstract void SetVelocity(Vector3 linear);

        /// <summary>물리 엔진이 굴리는 몸(다이나믹)의 결과를 World로 되읽을 때 쓴다.</summary>
        public abstract Vector3 GetPosition();

        public abstract Quaternion GetRotation();

        public abstract Vector3 GetVelocity();

        /// <summary>제자리에서 도는 몸은 선속도가 0이라, 멎었는지 보려면 이것도 봐야 한다.</summary>
        public abstract Vector3 GetAngularVelocity();

        /// <summary>몸을 어딘가로 되돌릴 때 회전 관성까지 지워야 그 자리에 선다.</summary>
        public abstract void SetAngularVelocity(Vector3 angular);

        /// <summary>
        /// 월드 좌표 한 점에 순간 충격(임펄스)을 준다. 몸 중심에서 벗어난 지점이면 회전이 함께 생긴다.
        /// 물리 엔진이 굴리는 몸(다이나믹)에만 의미가 있다.
        /// </summary>
        public abstract void AddImpulseAtPosition(Vector3 impulse, Vector3 worldPoint);

        /// <summary>
        /// 지정 레이어와의 겹침을 밀어낼 벡터(겹침 없으면 0). 위치 반영은 호출부 몫.
        /// 겹침을 판정할 포즈를 인자로 받는다 — 엔진 쪽 트랜스폼은 물리 스텝 뒤에야 갱신돼
        /// 진실원본(World.Transform)보다 늦고 롤백 재생 중에는 얼어 있다.
        /// </summary>
        public abstract Vector3 ComputePushOut(Vector3 position, Quaternion rotation, int layerMask);
    }
}
