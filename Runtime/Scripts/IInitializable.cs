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
}
