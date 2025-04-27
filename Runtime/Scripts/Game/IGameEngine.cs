using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGameEngine : IInitializableAsync, IDeinitializableAsync
    {
        IEntityManager entityManager { get; }
        ITickUpdater tickUpdater { get; }

        void Run(long tick, double interval, double elapsedTime);
        void UpdateEngine();

        void AddListener(object listener);
        void RemoveListener(object listener);
        void DispatchEvent<T>();
    }
}
