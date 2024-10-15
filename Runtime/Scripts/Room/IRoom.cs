using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IRoom : IInitializableAsync, IDeinitializableAsync
    {
        IGame game { get; }

        void StartGame();
    }
}
