using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IRunner : IInitializableAsync, IDeinitializableAsync
    {
        event Action<IGameState> onGameStateChanged;

        IGameState gameState { get; }

        ITickUpdater tickUpdater { get; }
        INetworkTime networkTime { get; }

        void Run(long tick, double interval, double elapsedTime);
        void Stop();
        void UpdateRunner();

        void AddListener(object listener);
        void RemoveListener(object listener);
        void DispatchEvent<T>();
    }
}
