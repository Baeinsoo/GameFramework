using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{
    public class GameEngineBase : MonoBehaviour, IGameEngine
    {
        public ITickUpdater tickUpdater { get; private set; }
        public IGameSystem[] gameSystems { get; private set; }

        public bool initialized { get; protected set; }

        public virtual async Task InitializeAsync()
        {
            tickUpdater = GetComponent<ITickUpdater>() ?? throw new ArgumentNullException(nameof(ITickUpdater));
            tickUpdater.onTick += OnTick;
            gameSystems = GetComponents<IGameSystem>();

            initialized = true;
        }

        public virtual async Task DeinitializeAsync()
        {
            tickUpdater.onTick -= OnTick;
            tickUpdater = null;
            gameSystems = null;

            initialized = false;
        }

        private void OnTick(long tick)
        {
            UpdateGame();
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

        public void UpdateGame()
        {
            foreach (var gameSystem in gameSystems.OrEmpty())
            {
                gameSystem.Update();
            }
        }
    }
}
