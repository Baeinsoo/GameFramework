using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{
    public abstract class GameEngineBase : MonoBehaviour, IGameEngine
    {
        public IEntityManager entityManager { get; private set; }
        public ITickUpdater tickUpdater { get; private set; }

        public bool initialized { get; protected set; }

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
    }
}
