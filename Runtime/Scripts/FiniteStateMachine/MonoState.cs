using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class MonoState : MonoBehaviour, IState
    {
        public IFiniteStateMachine FSM => gameObject.GetOrAddComponent<MonoStateMachine>();

        public virtual void OnEnter() { }
        public virtual void OnExit() { }

        public abstract IState GetNext<I>(I input) where I : Enum;
    }
}
