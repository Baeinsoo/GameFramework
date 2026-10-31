using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 내가 조종하는 몸의 예측 오차를 화면에서 녹인다. 소스 엔진 <c>cl_smooth</c>
    /// (<c>C_BasePlayer::NotePredictionError</c> / <c>GetPredictionErrorSmoothingVector</c>)와 같은 방식이다:
    /// 보정이 오면 남은 오차에 새 오차를 더하고, 그 합을 <see cref="_smoothTime"/> 동안 일정한 비율로 0까지 줄인다.
    ///
    /// 남의 몸용 <see cref="RenderCorrectionSmoother"/>와 다른 점: 화면 속도를 이어 붙이지 않는다. 속도를 물고
    /// 다시 시작하면 연달아 맞을 때 시뮬 자리를 지나쳤다 돌아와 출렁이고, 목줄·즉시 채택 문턱이 남의 새에
    /// 맞춰져 있어 판 후반 큰 넉백(5m 넘게)을 자른다. 내 몸은 손과 화면이 어긋나는 시간을 짧고 예측 가능하게
    /// 두는 쪽이 낫다.
    /// 순수(System.Numerics) — UnityEngine 타입을 들이지 않는다.
    /// </summary>
    public class PredictionErrorSmoother : IRenderCorrectionSmoother
    {
        private readonly float _smoothTime;
        private readonly float _minCorrection;      // 이보다 작으면 숨길 튐이 없다 → 바로 맞춘다
        private readonly float _teleportDistance;   // 이보다 크면 녹이지 않고 바로 간다(유니티 넷코드 maxDist와 같은 역할)

        private Vector3 _error;     // 보정 순간의 오차(그리던 자리 − 새 시뮬 자리)
        private float _elapsed;
        private bool _hasTarget;

        public PredictionErrorSmoother(float smoothTime, float minCorrection, float teleportDistance)
        {
            _smoothTime = smoothTime;
            _minCorrection = minCorrection;
            _teleportDistance = teleportDistance;
        }

        private Vector3 Remaining()
        {
            if (_smoothTime <= 0f || _elapsed >= _smoothTime)
            {
                return Vector3.Zero;
            }
            return _error * (1f - _elapsed / _smoothTime);
        }

        public Vector3 Target(Vector3 simPosition)
        {
            _hasTarget = true;
            return simPosition + Remaining();
        }

        public void OnCorrection(Vector3 oldSimPosition, Vector3 newSimPosition, Vector3 newSimVelocity, float deltaTime)
        {
            //  아직 한 번도 안 그렸으면 이어 줄 화면이 없다(스폰 직후 보정이 먼저 올 수 있다).
            if (_hasTarget == false || _smoothTime <= 0f)
            {
                Clear();
                return;
            }

            //  지금 그리던 자리(옛 시뮬 + 남은 오차)에서 출발한다.
            Vector3 error = Remaining() + (oldSimPosition - newSimPosition);
            float length = error.Length();
            if (length < _minCorrection || length > _teleportDistance)
            {
                Clear();
                return;
            }

            _error = error;
            _elapsed = deltaTime;
        }

        public void OnTeleport()
        {
            Clear();
        }

        public void Advance(float deltaTime)
        {
            if (_elapsed < _smoothTime)
            {
                _elapsed += deltaTime;
            }
        }

        public void Reset()
        {
            Clear();
            _hasTarget = false;
        }

        private void Clear()
        {
            _error = Vector3.Zero;
            _elapsed = _smoothTime;
        }
    }
}
