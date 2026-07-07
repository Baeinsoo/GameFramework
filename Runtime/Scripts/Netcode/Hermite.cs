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
    }
}
