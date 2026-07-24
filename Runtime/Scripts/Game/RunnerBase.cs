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

        private Dictionary<Type, Dictionary<object, Action>> listenerMap = new Dictionary<Type, Dictionary<object, Action>>();

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
    }
}
