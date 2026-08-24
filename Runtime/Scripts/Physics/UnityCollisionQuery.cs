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
                return new CollisionHit(true, hit.distance, hit.normal, hit.point, hit.collider);
            }
            return CollisionHit.None;
        }

        public CollisionHit Raycast(Vector3 origin, Vector3 direction, float distance, int layerMask)
        {
            // 트리거를 무시하는 이유는 CapsuleCast와 같다 — 통과해야 할 것에 막히면 안 된다.
            if (UnityEngine.Physics.Raycast(origin, direction, out RaycastHit hit,
                    distance, layerMask, QueryTriggerInteraction.Ignore))
            {
                return new CollisionHit(true, hit.distance, hit.normal, hit.point, hit.collider);
            }
            return CollisionHit.None;
        }

        public CollisionHit[] OverlapSphere(Vector3 center, float radius, int layerMask)
        {
            Collider[] colliders = UnityEngine.Physics.OverlapSphere(center, radius, layerMask,
                QueryTriggerInteraction.Ignore);

            var hits = new CollisionHit[colliders.Length];
            for (int i = 0; i < colliders.Length; i++)
            {
                // 겹침 검사는 접촉 지점을 주지 않는다 — 좌표 자리는 비워 두고 콜라이더만 싣는다.
                hits[i] = new CollisionHit(true, 0f, Vector3.zero, Vector3.zero, colliders[i]);
            }
            return hits;
        }
    }
}
