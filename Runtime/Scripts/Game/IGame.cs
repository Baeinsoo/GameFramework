using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGame
    {
        ITickUpdater tickUpdater { get; }

        void Initialize();
        void Deinitialize();

        void Run(long tick, double interval, double elapsedTime);
        void Stop();

        void OnTick(long tick);
    }
}
