using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>두 스냅(위치+속도) 사이 큐빅 Hermite 위치 보간. 순수 — 프레임독립·EditMode 테스트.</summary>
    public static class Hermite
    {
        /// <param name="dt">구간 길이(초) = newerTime − olderTime. 속도(탄젠트)를 위치 단위로 스케일.</param>
        /// <param name="u">정규화 파라미터 [0,1].</param>
        public static Vector3 Position(Vector3 p0, Vector3 v0, Vector3 p1, Vector3 v1, float dt, float u)
        {
            float u2 = u * u;
            float u3 = u2 * u;
            float h00 = 2f * u3 - 3f * u2 + 1f;
            float h10 = u3 - 2f * u2 + u;
            float h01 = -2f * u3 + 3f * u2;
            float h11 = u3 - u2;
            return h00 * p0 + h10 * dt * v0 + h01 * p1 + h11 * dt * v1;
        }

        /// <summary>
        /// <see cref="Position"/> 곡선의 그 지점 속도(초당). 위치 곡선을 미분한 값이라 실제로 보이는
        /// 움직임과 정확히 일치한다 — 끝점 속도를 선형으로 섞으면 곡선이 휘는 구간에서 어긋난다.
        /// </summary>
        /// <param name="dt">구간 길이(초) = newerTime − olderTime. 0 이하면 <paramref name="v0"/>를 돌려준다.</param>
        /// <param name="u">정규화 파라미터 [0,1].</param>
        public static Vector3 Velocity(Vector3 p0, Vector3 v0, Vector3 p1, Vector3 v1, float dt, float u)
        {
            if (dt <= 0f)
            {
                return v0;
            }
            float u2 = u * u;
            float dh00 = 6f * u2 - 6f * u;
            float dh10 = 3f * u2 - 4f * u + 1f;
            float dh01 = -6f * u2 + 6f * u;
            float dh11 = 3f * u2 - 2f * u;
            // du 기준 기울기를 초 단위로 환산(÷dt). p 항의 dt 스케일이 그대로 남아 v0/v1 항은 나누어 상쇄된다.
            return (dh00 * p0 + dh01 * p1) / dt + dh10 * v0 + dh11 * v1;
        }
    }
}
