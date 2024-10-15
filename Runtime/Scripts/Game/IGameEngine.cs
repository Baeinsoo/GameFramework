using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGameEngine : IInitializableAsync, IDeinitializableAsync
    {
        IGameSystem[] gameSystems { get; }
        ITickUpdater tickUpdater { get; }

        void Run(long tick, double interval, double elapsedTime);
    }
}
