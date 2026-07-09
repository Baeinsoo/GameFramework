using UnityEngine;

namespace GameFramework
{
    /// <summary>충돌 쿼리 결과(엔진 RaycastHit을 포트 경계에서 격리한 얇은 값 타입).</summary>
    public readonly struct CollisionHit
    {
        public readonly bool HasHit;
        public readonly float Distance;
        public readonly Vector3 Normal;
        public readonly Vector3 Point;

        public CollisionHit(bool hasHit, float distance, Vector3 normal, Vector3 point)
        {
            HasHit = hasHit;
            Distance = distance;
            Normal = normal;
            Point = point;
        }

        public static CollisionHit None => new CollisionHit(false, 0f, Vector3.zero, Vector3.zero);
    }
}
