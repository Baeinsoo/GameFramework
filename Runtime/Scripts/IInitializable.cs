using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IInitializableBase
    {
        bool initialized { get; }
    }

    public interface IInitializable : IInitializableBase
    {
        void Initialize();
    }

    public interface IInitializable<T1, T2, T3> : IInitializableBase
    {
        void Initialize(T1 value1, T2 value2, T3 value3);
    }
}
