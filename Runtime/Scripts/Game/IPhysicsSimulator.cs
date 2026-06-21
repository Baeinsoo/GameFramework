namespace GameFramework
{
    /// <summary>
    /// 물리 시뮬레이션 한 스텝을 구동하는 포트 추상. 엔진 본체가 구체 물리 엔진(PhysX 등)에
    /// 직결되지 않도록 주입한다. 클·서 양쪽 동일 구체(<see cref="UnityPhysicsSimulator"/>)를 사용한다.
    /// </summary>
    public interface IPhysicsSimulator
    {
        /// <summary>지정한 delta time만큼 물리 시뮬레이션을 한 스텝 진행한다.</summary>
        void Simulate(float deltaTime);
    }
}
