using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGame : IInitializableAsync, IDeinitializable
    {
        IGameEngine gameEngine { get; }

        void Run(long tick, double interval, double elapsedTime);
        void Stop();
    }
}
