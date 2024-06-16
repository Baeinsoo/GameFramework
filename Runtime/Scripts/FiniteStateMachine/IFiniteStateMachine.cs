using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IFiniteStateMachine
    {
        event Action<IState, IState> onStateChange;

        IState initState { get; }
        IState currentState { get; }

        void ProcessInput<I>(I input) where I : Enum;
    }
}
