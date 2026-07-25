using System;
using System.Collections;
using UnityEngine;

namespace GameFramework.Runner
{
    public class TickUpdaterBase : MonoBehaviour, ITickUpdater
    {
        // 프레임당 캐치업 상한. 50Hz 기준 최대 160ms. 초과분은 다음 프레임으로 이월.
        // 지속적으로 프레임이 느린(과부하) 호스트에선 틱 시계가 실시간보다 계속 뒤처진다 —
        // "멈춤(freeze)"을 "시간 뒤처짐"으로 맞바꾼 의도된 완만한 열화다. 뒤처짐을 앞으로
        // 건너뛰는 snap-forward는 Stage④(reconciliation) 몫이라 여기선 하지 않는다.
        private const int MaxTicksPerFrame = 8;

        public event Action<long> onTick;

        public long tick { get; private set; }
        public double interval { get; private set; }
        public double elapsedTime { get; protected set; }

        public long processibleTick
        {
            get
            {
                var processibleTick = (long)(elapsedTime / interval);
                return processibleTick;
            }
        }

        // 첫 틱(0)엔 0, 이후엔 고정 간격.
        public double deltaTime => tick == 0 ? 0 : interval;

        private Coroutine loop;
        private bool loggedCatchUpWarning;

        public void Run(long tick, double interval, double elapsedTime)
        {
            this.tick = tick;
            this.interval = interval;
            this.elapsedTime = elapsedTime;

            if (loop != null)
            {
                StopCoroutine(loop);
            }
            loop = StartCoroutine(TickUpdateLoop());
        }

        public void Stop()
        {
            if (loop != null)
            {
                StopCoroutine(loop);
                loop = null;
            }
        }

        private IEnumerator TickUpdateLoop()
        {
            while (true)
            {
                long frameEnd = TickCatchUp.ClampTarget(tick, processibleTick, MaxTicksPerFrame);

                // 이번 프레임에 밀린 틱을 다 못 따라잡으면(상한에 걸리면) 1회만 경고.
                bool capped = frameEnd < processibleTick;
                if (capped)
                {
                    if (loggedCatchUpWarning == false)
                    {
                        Debug.LogWarning($"[TickUpdater] catch-up capped at {MaxTicksPerFrame} ticks/frame (behind by {processibleTick - tick}).");
                        loggedCatchUpWarning = true;
                    }
                }
                else
                {
                    loggedCatchUpWarning = false;
                }

                while (tick <= frameEnd)
                {
                    TickBody();
                }

                yield return null;

                OnElapsedTimeUpdate();
            }
        }

        private void TickBody()
        {
            onTick?.Invoke(tick);
            tick++;
        }

        protected virtual void OnElapsedTimeUpdate()
        {
            // 비네트워크(오프라인) 기본값. 클·서는 override해 네트워크 시간으로 대체한다.
            // 고정 틱 누적기라 smoothDeltaTime(평활 평균)이 아닌 실제 deltaTime을 쓴다.
            elapsedTime += UnityEngine.Time.deltaTime;
        }
    }
}
