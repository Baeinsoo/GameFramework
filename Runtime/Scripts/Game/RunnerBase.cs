using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using GameFramework.Netcode;

namespace GameFramework
{
    public abstract class RunnerBase : MonoBehaviour, IRunner
    {
        public event Action<IGameState> onGameStateChanged;

        private IGameState _gameState;
        public IGameState gameState
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
        public INetworkTime networkTime { get; private set; }

        public bool initialized { get; protected set; }

        private Dictionary<Type, Dictionary<object, Action>> listenerMap = new Dictionary<Type, Dictionary<object, Action>>();

        public virtual async Task InitializeAsync()
        {
            tickUpdater = GetComponent<ITickUpdater>() ?? throw new ArgumentNullException(nameof(ITickUpdater));
            tickUpdater.onTick += OnTick;
            networkTime = CreateNetworkTime();   // 서버=null(override 안 함), 클라=MirrorNetworkTime. tickUpdater와 달리 null 허용.

            initialized = true;
        }

        public virtual async Task DeinitializeAsync()
        {
            // teardown 시점에 정적 current를 정리한다 (Stop()은 일시정지라 current를 유지).
            if (Runner.current == this)
            {
                Runner.current = null;
            }

            tickUpdater.onTick -= OnTick;
            tickUpdater = null;
            networkTime = null;

            initialized = false;
        }

        public virtual void Run(long tick, double interval, double elapsedTime)
        {
            Runner.current = this;

            tickUpdater.Run(tick, interval, elapsedTime);
        }

        public virtual void Stop()
        {
            // 일시정지: 틱만 멈춘다. 정적 current는 유지(정지 중에도 Runner.Time 등이 유효해야 함).
            tickUpdater.Stop();
        }

        private void OnTick(long tick)
        {
            UpdateRunner();
        }

        public abstract void UpdateRunner();

        public virtual void AddListener(object listener)
        {
            var methods = listener.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods.OrEmpty())
            {
                var attribute = method.GetCustomAttribute<RunnerListenAttribute>();
                if (attribute == null)
                {
                    continue;
                }

                if (listenerMap.TryGetValue(attribute.type, out var listeners) == false)
                {
                    listeners = new Dictionary<object, Action>();
                    listenerMap[attribute.type] = listeners;
                }

                Action action = (Action)Delegate.CreateDelegate(typeof(Action), listener, method);
                listeners[listener] = action;
            }
        }

        public virtual void RemoveListener(object listener)
        {
            foreach (var listeners in listenerMap.Values)
            {
                listeners.Remove(listener);
            }
        }

        public void DispatchEvent<T>()
        {
            if (listenerMap.TryGetValue(typeof(T), out var listeners))
            {
                foreach (var action in listeners.Values)
                {
                    action.Invoke();
                }
            }
        }

        /// <summary>
        /// 사이드별 네트워크 시간 소스를 생성한다. 기본 null(서버 — 자기 권위 시간, Runner.NetworkTime 미사용).
        /// 클라가 override해 네트워크 동기 시간을 제공한다.
        /// </summary>
        protected virtual INetworkTime CreateNetworkTime() => null;
    }
}
