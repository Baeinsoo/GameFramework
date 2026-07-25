using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameFramework.Netcode;

namespace GameFramework
{
    public interface IRunner : IInitializableAsync, IDeinitializableAsync
    {
        event Action<RunnerState> onGameStateChanged;

        RunnerState gameState { get; }

        ITickUpdater tickUpdater { get; }
        INetworkTime networkTime { get; }

        void Run(long tick, double interval, double elapsedTime);
        void Stop();

        void RegisterSystem<TPhase>(ITickSystem system);
        void UnregisterSystem(ITickSystem system);
    }
}
