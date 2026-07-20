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

        /// <summary>지정 레이어와의 겹침을 밀어낼 벡터(겹침 없으면 0). 위치 반영은 호출부 몫.</summary>
        public abstract Vector3 ComputePushOut(int layerMask);
    }
}
