using System;
using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 서버 보정을 '보이는 위치'에서만 흡수한다. 시뮬(권위) 위치는 하드 보정 그대로 두고, 렌더가
    /// 시뮬 위에 오차 offset을 얹어 그린 뒤 그 offset을 0으로 몰아간다.
    ///
    /// offset은 <b>3차 에르미트</b>로 만든다 — 이음매에서 위치뿐 아니라 <b>속도까지</b> 이어지게
    /// 하려는 것이다(단, <see cref="_maxSmoothDistance"/> 목줄이 당겨지는 구간은 예외다 — 그 클램프는
    /// 위치만 맞출 뿐[C0] 속도까지 잇지는 않아[C1 아님], 클램프가 풀리는 순간 속도가 한 번 꺾인다).
    /// 지수 감쇠는 시작 기울기가 (갭 / 시간상수)라 갭이 클수록 화면이 튕겨 나갔다
    /// (실측: 4.788m 갭에서 47.9 m/s). 목표는 Murphy의 projective velocity blending과 같고
    /// (Believable Dead Reckoning for Networked Games, Game Engine Gems 2), 그 조건을 정확히
    /// 만족시키는 3차식이 아래 <see cref="Evaluate"/>다.
    ///
    /// 문턱 이름은 언리얼 UCharacterMovementComponent를 따른다. 순수(System.Numerics) — 프레임독립·유닛 테스트 가능.
    /// </summary>
    public class RenderCorrectionSmoother
    {
        //  0이면 스무딩을 아예 안 한다(언리얼 NetworkSmoothingMode.Disabled). 내가 조종하는 몸이
        //  그렇다 — 녹이는 동안 입력과 화면이 어긋나 조작감이 무너진다.
        private readonly float _smoothTime;
        private readonly float _minCorrection;      // 이보다 작으면 숨길 튐이 없다 → 즉시 채택
        private readonly float _maxSmoothDistance;  // 보간 도중 렌더가 sim에서 이 이상 뒤처지지 않게 붙잡는다
        private readonly float _noSmoothDistance;   // 이보다 크면 녹이지 않고 즉시 채택

        private Vector3 _errorStart;        // E(0) — 보정 순간의 위치 갭
        private Vector3 _errorSlopeStart;   // E'(0)·T — 위치와 같은 단위로 미리 곱해 둔다
        private float _elapsed;             // 보정 후 경과(초)
        private bool _smoothing;

        private Vector3 _lastTarget;        // 마지막으로 낸 렌더 위치
        private Vector3 _prevTarget;        // 그 직전 것 — 둘의 차로 렌더 속도를 구한다
        private Vector3 _renderVelocity;
        private bool _hasTarget;
        private bool _hasPrev;

        public RenderCorrectionSmoother(float smoothTime, float minCorrection,
                                        float maxSmoothDistance, float noSmoothDistance)
        {
            _smoothTime = smoothTime;
            _minCorrection = minCorrection;
            _maxSmoothDistance = maxSmoothDistance;
            _noSmoothDistance = noSmoothDistance;
        }

        /// <summary>이번 프레임 렌더 위치 = 시뮬 위치 + 남은 오차.</summary>
        public Vector3 Target(Vector3 simPosition)
        {
            Vector3 error = Vector3.Zero;
            if (_smoothing)
            {
                error = Evaluate(_elapsed / _smoothTime);

                //  목줄: 보간 중이라도 이 이상은 뒤처지지 않는다.
                float lag = error.Length();
                if (lag > _maxSmoothDistance)
                {
                    error *= _maxSmoothDistance / lag;
                }
            }

            _lastTarget = simPosition + error;
            _hasTarget = true;
            return _lastTarget;
        }

        /// <summary>
        /// 서버 보정이 시뮬 위치를 옮겼음을 알린다. 렌더는 <paramref name="oldSimPosition"/> 쪽에
        /// 잠시 머물렀다 새 자리로 이어 간다. <paramref name="newSimVelocity"/>는 권위 속도로,
        /// 이음매에서 속도를 잇는 데 쓴다. <paramref name="deltaTime"/>는 이 보정 직후 곧바로
        /// 이어질 이번 틱의 시간폭이다 — 실제 호출 순서(Reconcile→world.Tick→Target)상 이
        /// 보정 뒤 첫 <see cref="Target"/> 호출은 sim이 이미 한 틱 더 진행된 뒤에 오므로, 그
        /// 한 틱을 미리 셈해 두지 않으면 이음매가 한 틱 늦게 체결된다.
        /// </summary>
        public void OnCorrection(Vector3 oldSimPosition, Vector3 newSimPosition, Vector3 newSimVelocity, float deltaTime)
        {
            //  아직 한 프레임도 안 그렸으면 이을 과거가 없다.
            if (_hasTarget == false || _smoothTime <= 0f)
            {
                _smoothing = false;
                _elapsed = 0f;
                return;
            }

            float gap = Vector3.Distance(oldSimPosition, newSimPosition);

            //  숨길 튐이 없을 만큼 작은 보정. 지금 아무것도 안 녹고 있으면 그냥 무시해 다음
            //  Target이 sim을 즉시 따르게 두고(=즉시 채택), 한창 녹고 있으면 그 진행 중인
            //  블렌드를 절대 건드리지 않는다 — 여기서 무조건 _smoothing을 꺼 버리면(예전 버그),
            //  활공만 하는 남의 새처럼 매 틱 gap=0으로 들어오는 호출 하나가 진행 중이던 훨씬
            //  큰 보정(수 미터 잔여 오차)을 한 프레임에 통째로 지워 순간이동으로 보이게 만든다.
            if (gap < _minCorrection)
            {
                return;
            }

            if (gap > _noSmoothDistance)
            {
                _smoothing = false;
                _elapsed = 0f;
                return;
            }

            _errorStart = _lastTarget - newSimPosition;
            //  E'(0) = (렌더가 가던 속도) − (권위 속도). 시간 단위를 없애려 T를 곱해 둔다.
            _errorSlopeStart = (_renderVelocity - newSimVelocity) * _smoothTime;
            _elapsed = deltaTime;
            _smoothing = true;
        }

        /// <summary>한 프레임 진행. 렌더 속도도 여기서 갱신한다(다음 보정의 이음매에 쓴다).</summary>
        public void Advance(float deltaTime)
        {
            if (_hasTarget && _hasPrev && deltaTime > 0f)
            {
                _renderVelocity = (_lastTarget - _prevTarget) / deltaTime;
            }
            if (_hasTarget)
            {
                _prevTarget = _lastTarget;
                _hasPrev = true;
            }

            if (_smoothing == false)
            {
                return;
            }
            _elapsed += deltaTime;
            if (_elapsed >= _smoothTime)
            {
                _smoothing = false;
            }
        }

        public void Reset()
        {
            _smoothing = false;
            _elapsed = 0f;
            _errorStart = Vector3.Zero;
            _errorSlopeStart = Vector3.Zero;
            _lastTarget = Vector3.Zero;
            _prevTarget = Vector3.Zero;
            _renderVelocity = Vector3.Zero;
            _hasTarget = false;
            _hasPrev = false;
        }

        //  E(0)=시작오차, E'(0)=시작기울기, E(1)=0, E'(1)=0 을 만족하는 유일한 3차식.
        //  끝에서 기울기까지 0이라 보간이 끝나는 순간에도 속도가 안 튄다.
        private Vector3 Evaluate(float u)
        {
            if (u <= 0f)
            {
                return _errorStart;
            }
            if (u >= 1f)
            {
                return Vector3.Zero;
            }
            float u2 = u * u;
            float u3 = u2 * u;
            return _errorStart * (2f * u3 - 3f * u2 + 1f)
                 + _errorSlopeStart * (u3 - 2f * u2 + u);
        }
    }
}
