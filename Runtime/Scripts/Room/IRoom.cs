using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{
    public interface IRoom : IInitializableAsync, IDeinitializableAsync
    {
        IGame game { get; }

        Task StartGame();
    }
}
