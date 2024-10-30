using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGame : IInitializableAsync, IDeinitializableAsync
    {
        event Action<GameState> onGameStateChanged;

        GameState gameState { get; }
        IGameEngine gameEngine { get; }

        void Run(long tick, double interval, double elapsedTime);
    }
}
