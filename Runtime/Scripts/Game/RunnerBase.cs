using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GameFramework.Netcode;

namespace GameFramework
{
    public abstract class RunnerBase : MonoBehaviour, IRunner
    {
        public event Action<RunnerState> onGameStateChanged;

        private RunnerState _gameState;
        public RunnerState gameState
        {
            get => _gameState;
            // 구체 상태 값은 use-side(LOP)가 정의하고 전이한다. 베이스는 마커 보관·발화만 한다.
            protected set
            {
                if (_gameState == value)
                {
                    return;
                }

                _gameState = value;
                onGameStateChanged?.Invoke(value);
            }
        }

        public ITickUpdater tickUpdater { get; private set; }
        public INetworkTime networkTime { get; protected set; }

        public bool initialized { get; protected set; }

        private readonly Dictionary<Type, List<ITickSystem>> _tickSystems = new Dictionary<Type, List<ITickSystem>>();

        public void RegisterSystem<TPhase>(ITickSystem system)
        {
            var key = typeof(TPhase);
            if (_tickSystems.TryGetValue(key, out var list) == false)
            {
                list = new List<ITickSystem>();
                _tickSystems[key] = list;
            }
            // 중복 등록 방지(구 리스너 맵의 idempotency 유지) — 같은 인스턴스가 두 번 틱하지 않게.
            if (list.Contains(system) == false)
            {
                list.Add(system);
            }
        }

        public void UnregisterSystem(ITickSystem system)
        {
            foreach (var list in _tickSystems.Values)
            {
                list.Remove(system);
            }
        }

        // 페이즈에 등록된 시스템을 등록 순서대로 실행(추가 할당 없음).
        // 불변식: 시스템은 자기 Tick 도중 UnregisterSystem을 호출하지 않는다 —
        // 엔티티 사망→Cleanup(=해제)은 파이프라인의 별도 스텝(FlushDespawns)으로 지연되므로
        // 이 순회 중 list는 변하지 않는다. (이 전제가 깨지면 스냅샷 순회로 바꿔야 함.)
        protected void RunPhase<TPhase>(long tick, float deltaTime)
        {
            if (_tickSystems.TryGetValue(typeof(TPhase), out var list) == false)
            {
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Tick(tick, deltaTime);
            }
        }

        public virtual async Task InitializeAsync()
        {
            tickUpdater = GetComponent<ITickUpdater>() ?? throw new ArgumentNullException(nameof(ITickUpdater));
            tickUpdater.onTick += OnTick;

            initialized = true;
        }

        public virtual async Task DeinitializeAsync()
        {
            tickUpdater.onTick -= OnTick;
            tickUpdater = null;
            networkTime = null;

            initialized = false;
        }

        public virtual void Run(long tick, double interval, double elapsedTime)
        {
            tickUpdater.Run(tick, interval, elapsedTime);
        }

        public virtual void Stop()
        {
            // 일시정지: 틱만 멈춘다.
            tickUpdater.Stop();
        }

        private void OnTick(long tick)
        {
            UpdateRunner();
        }

        public abstract void UpdateRunner();
    }
}
