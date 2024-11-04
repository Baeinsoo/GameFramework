using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IGamePresenter
    {
        IGame game { get; }
    }
}
