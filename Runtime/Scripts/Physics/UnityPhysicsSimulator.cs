using UnityEngine;

namespace GameFramework.Physics
{
    /// <summary>
    /// Unity 내장 물리(PhysX)로 <see cref="IPhysicsSimulator"/>를 구현하는 어댑터.
    /// <c>SimulationMode.Script</c> 설정은 호출자(LOPGame) 책임 — 이 어댑터는 한 스텝 실행만 위임한다.
    /// </summary>
    public sealed class UnityPhysicsSimulator : IPhysicsSimulator
    {
        public void Simulate(float deltaTime) => UnityEngine.Physics.Simulate(deltaTime);
    }
}
