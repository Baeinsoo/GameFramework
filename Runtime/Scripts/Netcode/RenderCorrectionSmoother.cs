using System;
using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 서버 보정을 '보이는 위치'에서만 부드럽게 흡수한다. 시뮬(권위) 위치는 하드 보정 그대로 두고,
    /// 렌더 위치가 (보이던 위치 − 새 권위 위치)만큼의 오차 offset을 잡은 뒤 0으로 감쇠시켜 새 위치로
    /// 수렴한다 — Unreal MeshTranslationOffset / Fiedler render=simPos+errorOffset 대응.
    /// 순수(System.Numerics) — 프레임독립·유닛 테스트 가능.
    /// </summary>
    public class RenderCorrectionSmoother
    {
        private readonly float _tau;             // 감쇠 시간상수(초) — 클수록 천천히 녹음
        private readonly float _minCorrection;   // 이보다 작은 보정은 무시(즉시 채택)
        private readonly float _teleport;        // 이보다 큰 보정은 스무딩 안 함(즉시 스냅)

        private Vector3 _offset;                 // 렌더 = simPos + _offset
        private Vector3 _lastTarget;             // 마지막으로 낸 렌더 위치(보정 seed 기준)
        private bool _hasTarget;

        public RenderCorrectionSmoother(float tau, float minCorrection, float teleport)
        {
            _tau = tau;
            _minCorrection = minCorrection;
            _teleport = teleport;
        }

        /// <summary>이번 틱 렌더 위치 = simPos + offset. 낸 값을 seed 기준으로 캐시한다.</summary>
        public Vector3 Target(Vector3 simPos)
        {
            _lastTarget = simPos + _offset;
            _hasTarget = true;
            return _lastTarget;
        }

        /// <summary>
        /// 서버 보정 발생. 보정 크기가 밴드 안이면 렌더를 직전에 보이던 자리에 붙들어 두는 offset을
        /// 잡고(이후 감쇠로 새 위치에 수렴), 너무 작거나(무시) 너무 크면(리스폰 등) offset을 0으로 두어
        /// 렌더가 즉시 새 위치를 따르게 한다.
        /// </summary>
        public void OnCorrection(Vector3 oldSimPos, Vector3 newSimPos)
        {
            if (!_hasTarget)
            {
                _offset = Vector3.Zero;
                return;
            }
            float mag = Vector3.Distance(oldSimPos, newSimPos);
            if (mag < _minCorrection || mag > _teleport)
            {
                _offset = Vector3.Zero;
            }
            else
            {
                _offset = _lastTarget - newSimPos;
            }
        }

        /// <summary>offset을 0으로 한 틱만큼 지수 감쇠(프레임독립).</summary>
        public void DecayTick(float deltaTime)
        {
            _offset *= MathF.Exp(-deltaTime / _tau);
        }

        public void Reset()
        {
            _offset = Vector3.Zero;
            _lastTarget = Vector3.Zero;
            _hasTarget = false;
        }
    }
}
