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

        // 순회 전 스냅샷용 재사용 버퍼(RunPhase 공통). RunPhase는 protected라 시스템이 재진입
        // 호출할 수 없어(시스템은 RunnerBase 비상속) 버퍼 하나를 공유해도 안전하다.
        private readonly List<ITickSystem> _runBuffer = new List<ITickSystem>();

        // 페이즈 시스템을 등록 순서대로 실행. 순회 전 재사용 버퍼로 스냅샷을 떠서 —
        // Tick 도중 원본 list가 (해제/등록으로) 바뀌어도 안전하고, 순회 순서도 흔들리지 않는다.
        // 버퍼 재사용이라 최초 1회(최대 페이즈 크기)만 커지고 이후엔 추가 할당 없음(GC 거의 0).
        protected void RunPhase<TPhase>(long tick, float deltaTime)
        {
            if (_tickSystems.TryGetValue(typeof(TPhase), out var list) == false)
            {
                return;
            }
            _runBuffer.Clear();
            _runBuffer.AddRange(list);
            for (int i = 0; i < _runBuffer.Count; i++)
            {
                _runBuffer[i].Tick(tick, deltaTime);
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
