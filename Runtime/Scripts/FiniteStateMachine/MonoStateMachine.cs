using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class MonoStateMachine : MonoBehaviour, IFiniteStateMachine
    {
        public event Action<IState, IState> onStateChange;

        public abstract IState initState { get; }
        public IState currentState { get; private set; }

        public void StartStateMachine()
        {
            currentState = initState;
            currentState.Enter();
        }

        public void StopStateMachine()
        {
            currentState?.Exit();
            currentState = null;
        }

        public void ProcessInput<I>(I input) where I : Enum
        {
            var nextState = currentState.GetNext(input);

            if (currentState == nextState)
            {
                Debug.LogWarning($"nextState is same with currentState. nextState: {nextState}");
                return;
            }

            currentState.Exit();

            onStateChange?.Invoke(currentState, nextState);
            currentState = nextState;

            currentState.Enter();
        }
    }
}
