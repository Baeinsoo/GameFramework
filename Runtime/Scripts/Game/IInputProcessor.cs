using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IInputProcessor
    {
        void OnTick(long tick);
    }
}
