using UnityEngine;

namespace GameFramework.Physics
{
    /// <summary>
    /// 키네마틱 캡슐이 겹친 지오메트리 밖으로 나갈 push-out 벡터를 계산한다(자기 콜라이더 제외).
    /// sweep 이동은 "시작부터 겹친" 콜라이더를 무시하므로(스폰 flush·끼임), 이동 전 이 값으로 위치를 교정한다.
    /// 클·서 공유 — 호출부(UnityPhysicsBody)가 실제 콜라이더로 부른다: ComputePenetration이 self를
    /// 제외하려면 실제 콜라이더 참조가 필요해 포트(ICollisionQuery)로는 담기 어렵다.
    ///
    /// 캡슐이 **어디 있는지는 인자로 받는다** — 콜라이더에 딸린 엔진 트랜스폼을 읽으면 안 된다.
    /// 그 값은 물리 스텝이 지나야 갱신돼 진실원본(World.Transform)보다 한 틱 늦고, 클라 롤백
    /// 재생 중에는 물리를 돌리지 않으므로 아예 얼어 있다. 포즈를 받으면 이 계산이 "지금 상태 +
    /// 정적 맵"만의 순수 함수가 돼 라이브 틱과 재생 틱이 같은 답을 낸다.
    /// </summary>
    public static class KinematicDepenetration
    {
        /// <summary>겹침을 밀어낼 합산 벡터를 반환(겹침 없으면 <see cref="Vector3.zero"/>). 위치 반영은 호출부 몫.</summary>
        public static Vector3 ComputePushOut(CapsuleCollider own, Vector3 position, Quaternion rotation, int layerMask)
        {
            Vector3 center = position + rotation * own.center;
            float radius = own.radius;
            float halfSpan = Mathf.Max(own.height * 0.5f - radius, 0f);
            Vector3 up = rotation * Vector3.up;
            Vector3 p1 = center - up * halfSpan;
            Vector3 p2 = center + up * halfSpan;

            Collider[] overlaps = UnityEngine.Physics.OverlapCapsule(p1, p2, radius, layerMask, QueryTriggerInteraction.Ignore);
            Vector3 total = Vector3.zero;
            foreach (var other in overlaps)
            {
                if (other == own)
                {
                    continue;   // 자기 콜라이더 제외
                }
                if (UnityEngine.Physics.ComputePenetration(own, position, rotation,
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
