using UnityEngine;

namespace GameFramework.Physics
{
    /// <summary>Unity 내장 물리(PhysX)로 <see cref="ICollisionQuery"/>를 구현하는 어댑터.</summary>
    public sealed class UnityCollisionQuery : ICollisionQuery
    {
        public CollisionHit CapsuleCast(Vector3 point1, Vector3 point2, float radius,
            Vector3 direction, float distance, int layerMask)
        {
            // 이동 sweep은 트리거(아이템 픽업 등)에 막히면 안 된다 → 트리거 무시.
            if (UnityEngine.Physics.CapsuleCast(point1, point2, radius, direction, out RaycastHit hit,
                    distance, layerMask, QueryTriggerInteraction.Ignore))
            {
                return new CollisionHit(true, hit.distance, hit.normal, hit.point);
            }
            return CollisionHit.None;
        }
    }
}
