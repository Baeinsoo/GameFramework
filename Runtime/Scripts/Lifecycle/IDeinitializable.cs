using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

namespace GameFramework
{
    public interface IDeinitializable
    {
        void Deinitialize();
    }

    public interface IDeinitializableAsync
    {
        Task DeinitializeAsync();
    }
}
