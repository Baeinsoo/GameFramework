using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 서버 보정으로 시뮬 위치가 옮겨졌을 때, 화면에 그릴 위치만 잠시 옛 자리 쪽에 두었다가 따라가게 한다.
    /// 시뮬(권위) 위치는 건드리지 않는다. 남의 몸용(<see cref="RenderCorrectionSmoother"/>)과
    /// 내 몸용(<see cref="PredictionErrorSmoother"/>)이 있다.
    /// </summary>
    public interface IRenderCorrectionSmoother
    {
        /// <summary>이번에 그릴 위치 = 시뮬 위치 + 남은 오차.</summary>
        Vector3 Target(Vector3 simPosition);

        /// <summary>
        /// 시뮬 위치가 <paramref name="oldSimPosition"/>에서 <paramref name="newSimPosition"/>으로 옮겨졌다.
        /// <paramref name="deltaTime"/>는 이 보정 직후 곧바로 이어질 한 틱의 시간폭이다(그만큼 미리 센다).
        /// </summary>
        void OnCorrection(Vector3 oldSimPosition, Vector3 newSimPosition, Vector3 newSimVelocity, float deltaTime);

        /// <summary>의도된 순간이동 — 녹이지 않고 바로 간다.</summary>
        void OnTeleport();

        void Advance(float deltaTime);

        void Reset();
    }
}
