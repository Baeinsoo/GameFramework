using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IState
    {
        IFiniteStateMachine FSM { get; }

        void OnEnter();
        void OnExit();

        IState GetNext<I>(I input) where I : Enum;
    }
}
