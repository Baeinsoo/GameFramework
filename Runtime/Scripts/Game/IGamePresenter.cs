using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGamePresenter<T> where T : IGame 
    {
        T game { get; }
    }
}
