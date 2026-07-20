using System;
using System.Collections.Generic;

namespace GameFramework
{
    public abstract class StateMachine<TEvent> : IStateMachine<TEvent> where TEvent : struct, Enum
    {
        public event Action<IState<TEvent>, IState<TEvent>> onStateChange;

        public abstract IState<TEvent> initState { get; }
        public IState<TEvent> currentState { get; private set; }

        private readonly Queue<TEvent> pendingEvents = new Queue<TEvent>();
        private bool isProcessing;

        public void Start()
        {
            TransitionTo(initState);
        }

        public void Stop()
        {
            TransitionTo(null);
        }

        public void Fire(TEvent ev)
        {
            //  전이 도중(Enter/onStateChange)에서 또 Fire되면 큐에 쌓아 순서대로 처리.
            pendingEvents.Enqueue(ev);

            if (isProcessing)
            {
                return;
            }

            isProcessing = true;
            try
            {
                while (pendingEvents.Count > 0)
                {
                    ProcessEvent(pendingEvents.Dequeue());
                }
            }
            finally
            {
                isProcessing = false;
            }
        }

        private void ProcessEvent(TEvent ev)
        {
            if (currentState == null)
            {
                return;
            }

            var nextState = currentState.GetNextState(ev);

            if (currentState == nextState)
            {
                UnityEngine.Debug.Log($"{GetType().Name}: ignored event '{ev}' in state '{currentState.GetType().Name}'.");
                return;
            }

            TransitionTo(nextState);
        }

        private void TransitionTo(IState<TEvent> state)
        {
            if (currentState == state)
            {
                return;
            }

            var previous = currentState;
            currentState = state;   //  Enter가 다시 Fire해도 currentState는 이미 새 상태.

            previous?.Exit();

            onStateChange?.Invoke(previous, state);

            if (currentState != null)
            {
                currentState.FSM = this;
                currentState.Enter();
            }
        }
    }
}
