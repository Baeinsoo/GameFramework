using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class MonoState : MonoBehaviour, IState
    {
        public IFiniteStateMachine FSM => gameObject.GetOrAddComponent<MonoStateMachine>();

        public void Enter()
        {
            OnEnter();
            StartCoroutine("OnExecute");
        }

        public void Exit()
        {
            StopCoroutine("OnExecute");
            OnExit();
        }

        public abstract IState GetNext<I>(I input) where I : Enum;

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual IEnumerator OnExecute()
        {
            yield break;
        }
    }
}
