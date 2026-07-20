using UnityEngine;

namespace GameFramework.Physics
{
    /// <summary>
    /// 물리 충돌 쿼리 포트. 캡슐을 쓸어(sweep) 첫 충돌을 찾는다. 엔진 물리(PhysX)에 직결되지
    /// 않도록 주입한다. <see cref="IPhysicsSimulator"/>(스텝 구동)와 짝을 이루는 쿼리 추상.
    /// 클·서 양쪽 동일 구체(<see cref="UnityCollisionQuery"/>)를 사용한다.
    /// </summary>
    public interface ICollisionQuery
    {
        /// <summary>
        /// 캡슐(양 끝 구 중심 <paramref name="point1"/>·<paramref name="point2"/>, 반지름
        /// <paramref name="radius"/>)을 <paramref name="direction"/> 방향으로
        /// <paramref name="distance"/>만큼 쓸어 첫 충돌을 반환한다. 없으면 <see cref="CollisionHit.None"/>.
        /// </summary>
        CollisionHit CapsuleCast(Vector3 point1, Vector3 point2, float radius,
            Vector3 direction, float distance, int layerMask);
    }
}
