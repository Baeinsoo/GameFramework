using UnityEngine;

namespace GameFramework.Physics
{
    /// <summary>
    /// 물리 충돌 쿼리 포트. 엔진 물리(PhysX)에 직결되지 않도록 주입한다.
    /// <see cref="IPhysicsSimulator"/>(스텝 구동)와 짝을 이루는 쿼리 추상.
    /// 클·서 양쪽 동일 구체(<see cref="UnityCollisionQuery"/>)를 사용한다 — 이동 판정이 양쪽에서
    /// 같아야 하기 때문이다.
    ///
    /// 메서드 이름은 <c>UnityEngine.Physics</c>에서 그대로 차용한다.
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

        /// <summary>
        /// <paramref name="origin"/>에서 <paramref name="direction"/> 방향으로
        /// <paramref name="distance"/>만큼 광선을 쏴 첫 충돌을 반환한다. 없으면 <see cref="CollisionHit.None"/>.
        /// </summary>
        CollisionHit Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask);

        /// <summary>
        /// 중심 <paramref name="center"/>·반지름 <paramref name="radius"/> 구에 겹치는 콜라이더들.
        /// 겹치는 게 없으면 빈 배열.
        ///
        /// ⚠️ 겹침 검사는 접촉 지점을 알려주지 않는다 — 반환된 히트의
        /// <see cref="CollisionHit.Point"/>·<see cref="CollisionHit.Normal"/>·
        /// <see cref="CollisionHit.Distance"/>는 **0으로 채워지며 의미가 없다.**
        /// <see cref="CollisionHit.Collider"/>만 유효하다.
        ///
        /// ⚠️ 한 엔티티가 콜라이더를 여럿 가질 수 있으므로 **같은 대상이 여러 번 나올 수 있다.**
        /// 중복 제거는 부르는 쪽 몫이다.
        /// </summary>
        CollisionHit[] OverlapSphere(Vector3 center, float radius, int layerMask);
    }
}
