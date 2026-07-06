using System;
using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 서버 보정 후 렌더 위치를 target으로 부드럽게 이징한다(타깃으로만 수렴 — 오버슈트 없음).
    /// 보정 창 동안만 지수 이징, 평소엔 target을 정확히 추종(랙 0). 시뮬(true)과 분리된 render 스무딩 —
    /// Unreal NetworkSmoothingMode(Exponential) 대응. 순수(System.Numerics) — 프레임독립·유닛 테스트 가능.
    /// </summary>
    public class RenderCorrectionSmoother
    {
        private readonly float _smoothingTime;
        private Vector3 _current;
        private bool _hasCurrent;
        private float _windowRemaining;

        public RenderCorrectionSmoother(float smoothingTime)
        {
            _smoothingTime = smoothingTime;
        }

        /// <summary>보정 발생 알림 — 이징 창을 연다(초). 평소엔 호출 안 됨 → 정확 추종.</summary>
        public void MarkCorrection(float window)
        {
            _windowRemaining = window;
        }

        /// <summary>이번 프레임 렌더 위치. 창 동안 target으로 지수 이징, 평소 target 그대로.</summary>
        public Vector3 Smooth(Vector3 target, float deltaTime)
        {
            if (!_hasCurrent)
            {
                _current = target;
                _hasCurrent = true;
                return _current;
            }
            if (_windowRemaining > 0f)
            {
                _windowRemaining -= deltaTime;
                float a = 1f - MathF.Exp(-deltaTime / _smoothingTime);
                _current = Vector3.Lerp(_current, target, a);
            }
            else
            {
                _current = target;
            }
            return _current;
        }

        public void Reset()
        {
            _hasCurrent = false;
            _windowRemaining = 0f;
        }
    }
}
