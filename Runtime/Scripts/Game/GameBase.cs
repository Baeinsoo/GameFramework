using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class GameBase : MonoBehaviour, IGame
    {
        public ITickUpdater tickUpdater { get; private set; }

        public virtual void Initialize()
        {
            this.tickUpdater = GetComponent<ITickUpdater>();
            this.tickUpdater.onTick += OnTick;
        }

        public virtual void Deinitialize()
        {
            tickUpdater.onTick -= OnTick;
            tickUpdater.Deinitialize();
            tickUpdater = null;
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
