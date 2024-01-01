using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGameProcessor
    {
        void OnTick(long tick);
    }
}
