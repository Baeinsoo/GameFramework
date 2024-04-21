using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class MonoStateMachine : MonoBehaviour, IFiniteStateMachine
    {
        public abstract IState initState { get; }
        public IState currentState { get; private set; }

        public void StartStateMachine()
        {
            currentState = initState;
            currentState.OnEnter();
        }

        public void StopStateMachine()
        {
            currentState?.OnExit();
            currentState = null;
        }

        public void MoveNext<I>(I input) where I : Enum
        {
            var nextState = currentState.GetNext(input);

            if (currentState == nextState)
            {
                Debug.LogWarning($"nextState is same with currentState. nextState: {nextState}");
                return;
            }

            currentState.OnExit();

            currentState = nextState;
            OnStateChange();

            currentState.OnEnter();
        }

        public virtual void OnStateChange() { }
    }
}
