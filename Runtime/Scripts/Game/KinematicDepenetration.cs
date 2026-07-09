using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 키네마틱 캡슐이 겹친 지오메트리 밖으로 나갈 push-out 벡터를 계산한다(자기 콜라이더 제외).
    /// sweep 이동은 "시작부터 겹친" 콜라이더를 무시하므로(스폰 flush·끼임), 이동 전 이 값으로 위치를 교정한다.
    /// 클·서 공유 — 호출부(PhysicsComponent)가 실제 콜라이더로 부른다: ComputePenetration이 self를
    /// 제외하려면 실제 콜라이더 참조가 필요해 포트(ICollisionQuery)로는 담기 어렵다.
    /// </summary>
    public static class KinematicDepenetration
    {
        /// <summary>겹침을 밀어낼 합산 벡터를 반환(겹침 없으면 <see cref="Vector3.zero"/>). 위치 반영은 호출부 몫.</summary>
        public static Vector3 ComputePushOut(CapsuleCollider own, int layerMask)
        {
            Vector3 ownPos = own.transform.position;
            Quaternion ownRot = own.transform.rotation;

            Vector3 center = own.transform.TransformPoint(own.center);
            float radius = own.radius;
            float halfSpan = Mathf.Max(own.height * 0.5f - radius, 0f);
            Vector3 up = own.transform.up;
            Vector3 p1 = center - up * halfSpan;
            Vector3 p2 = center + up * halfSpan;

            Collider[] overlaps = Physics.OverlapCapsule(p1, p2, radius, layerMask, QueryTriggerInteraction.Ignore);
            Vector3 total = Vector3.zero;
            foreach (var other in overlaps)
            {
                if (other == own)
                {
                    continue;   // 자기 콜라이더 제외
                }
                if (Physics.ComputePenetration(own, ownPos, ownRot,
                        other, other.transform.position, other.transform.rotation,
                        out Vector3 dir, out float dist))
                {
                    total += dir * dist;
                }
            }
            return total;
        }
    }
}
