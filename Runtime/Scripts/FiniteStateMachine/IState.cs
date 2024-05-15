using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IState
    {
        IFiniteStateMachine FSM { get; }

        void Enter();
        void Exit();

        IState GetNext<I>(I input) where I : Enum;
    }
}
