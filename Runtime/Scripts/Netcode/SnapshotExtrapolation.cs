using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 마지막으로 받은 상태에서 앞을 내다보는 순수 산수(dead reckoning 2차). 보간
    /// (<see cref="SnapshotInterpolation"/>)이 "받은 두 점 사이"를 그리는 것과 달리, 이쪽은
    /// "받은 마지막 점 너머"를 그린다 — 원격을 내 시각에 맞춰 놓을 때 쓴다.
    /// <para>가속도는 인자로 받는다. 중력 같은 값은 게임이 알고 커널은 모른다.</para>
    /// <para>순수(System.Numerics) — 프레임독립·유닛 테스트 가능. 소비자(엔진 타입 쪽)는
    /// <see cref="Hermite"/>를 부르는 <c>SnapshotEntityInterpolator</c>처럼 경계에서 변환한다.</para>
    /// </summary>
    public static class SnapshotExtrapolation
    {
        /// <summary>
        /// <paramref name="elapsed"/>초 뒤 위치. <paramref name="maxElapsed"/>를 넘는 경과는
        /// 상한에서 잘린다 — 오래 못 받을수록 틀리므로, 계속 내달리는 대신 그 자리에 세운다.
        /// </summary>
        public static Vector3 Position(Vector3 position, Vector3 velocity, Vector3 acceleration,
            float elapsed, float maxElapsed)
        {
            if (elapsed <= 0f)
            {
                return position;
            }
            float t = elapsed > maxElapsed ? maxElapsed : elapsed;
            return position + velocity * t + 0.5f * acceleration * (t * t);
        }
    }
}
