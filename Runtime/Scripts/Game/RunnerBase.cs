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
            list.Add(system);
        }

        public void UnregisterSystem(ITickSystem system)
        {
            foreach (var list in _tickSystems.Values)
            {
                list.Remove(system);
            }
        }

        // 페이즈에 등록된 시스템을 실행. 역방향 순회 = Tick 중 자기 해제(엔티티 사망→Cleanup)해도 안전.
        // 페이즈 내 순서엔 의존하지 않는다(각 페이즈 소비자 ≤1종 또는 순서 무관 AI).
        protected void RunPhase<TPhase>(long tick, float deltaTime)
        {
            if (_tickSystems.TryGetValue(typeof(TPhase), out var list) == false)
            {
                return;
            }
            for (int i = list.Count - 1; i >= 0; i--)
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
