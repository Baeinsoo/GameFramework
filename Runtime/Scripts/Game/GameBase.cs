using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class GameBase : MonoBehaviour, IGame
    {
        public ITickUpdater tickUpdater { get; private set; }
        public IInputProcessor inputProcessor { get; private set; }
        public IGameProcessor gameProcessor { get; private set; }

        public virtual void Initialize()
        {
            tickUpdater = GetComponent<ITickUpdater>() ?? throw new ArgumentNullException(nameof(ITickUpdater));
            tickUpdater.onTick += OnTick;

            inputProcessor = GetComponent<IInputProcessor>() ?? throw new ArgumentNullException(nameof(IInputProcessor));
            gameProcessor = GetComponent<IGameProcessor>() ?? throw new ArgumentNullException(nameof(IGameProcessor));
        }

        public virtual void Deinitialize()
        {
            tickUpdater.onTick -= OnTick;
            tickUpdater.Deinitialize();
            tickUpdater = null;

            inputProcessor = null;
            gameProcessor = null;
        }

        public void Run()
        {
            Run(0, 1 / 60.0, 0);
        }

        public void Run(long tick, double interval, double elapsedTime)
        {
            Game.current = this;

            tickUpdater.Run(tick, interval, elapsedTime);
        }

        public void Stop()
        {
            if (Game.current == this)
            {
                Game.current = null;
            }
        }

        public abstract void OnTick(long tick);
    }
}
