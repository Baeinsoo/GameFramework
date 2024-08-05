using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IInitializable
    {
        bool initialized { get; }

        void Initialize();
    }

    public interface IInitializable<T1> : IInitializable
    {
        void Initialize(T1 param);
    }

    public interface IInitializable<T1, T2> : IInitializable
    {
        void Initialize(T1 param1, T2 param2);
    }

    public interface IInitializable<T1, T2, T3> : IInitializable
    {
        void Initialize(T1 param1, T2 param2, T3 param3);
    }
}
