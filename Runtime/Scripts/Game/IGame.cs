using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGame : IInitializableAsync, IDeinitializableAsync
    {
        IGameEngine gameEngine { get; }

        event Action onGameEnd;

        void Run(long tick, double interval, double elapsedTime);
    }
}
