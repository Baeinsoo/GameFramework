using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{
    public abstract class GameEngineBase : MonoBehaviour, IGameEngine
    {
        public IEntityManager entityManager { get; private set; }
        public ITickUpdater tickUpdater { get; private set; }

        public bool initialized { get; protected set; }

        private Dictionary<Type, Dictionary<object, Action>> listenerMap = new Dictionary<Type, Dictionary<object, Action>>();

        public virtual async Task InitializeAsync()
        {
            tickUpdater = GetComponent<ITickUpdater>() ?? throw new ArgumentNullException(nameof(ITickUpdater));
            tickUpdater.onTick += OnTick;
            entityManager = GetComponent<IEntityManager>() ?? throw new ArgumentNullException(nameof(IEntityManager));

            initialized = true;
        }

        public virtual async Task DeinitializeAsync()
        {
            tickUpdater.onTick -= OnTick;
            tickUpdater = null;
            entityManager = null;

            initialized = false;
        }

        public void Run(long tick, double interval, double elapsedTime)
        {
            GameEngine.current = this;

            tickUpdater.Run(tick, interval, elapsedTime);
        }

        public void Stop()
        {
            if (GameEngine.current == this)
            {
                GameEngine.current = null;
            }

            tickUpdater.Stop();
        }

        private void OnTick(long tick)
        {
            UpdateEngine();
        }

        public abstract void UpdateEngine();

        public virtual void AddListener(object listener)
        {
            var methods = listener.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var method in methods.OrEmpty())
            {
                var attribute = method.GetCustomAttribute<GameEngineListenAttribute>();
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
