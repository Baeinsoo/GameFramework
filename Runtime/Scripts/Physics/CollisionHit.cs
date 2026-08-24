using UnityEngine;

namespace GameFramework.Physics
{
    /// <summary>
    /// 충돌 쿼리 결과(엔진 RaycastHit을 포트 경계에서 격리한 얇은 값 타입).
    /// <see cref="Collider"/>는 유니티 <c>RaycastHit.collider</c>와 같은 자리다 — 맞은 것이 게임의
    /// 무엇인지는 이 계층이 알지 않고, 부르는 쪽이 이걸로 되짚는다.
    /// </summary>
    public readonly struct CollisionHit
    {
        public readonly bool HasHit;
        public readonly float Distance;
        public readonly Vector3 Normal;
        public readonly Vector3 Point;
        public readonly Collider Collider;

        public CollisionHit(bool hasHit, float distance, Vector3 normal, Vector3 point, Collider collider)
        {
            HasHit = hasHit;
            Distance = distance;
            Normal = normal;
            Point = point;
            Collider = collider;
        }

        public static CollisionHit None => new CollisionHit(false, 0f, Vector3.zero, Vector3.zero, null);
    }
}
