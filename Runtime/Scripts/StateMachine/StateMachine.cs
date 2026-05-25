using System;

namespace GameFramework
{
    public abstract class StateMachine<TEvent> : IStateMachine<TEvent> where TEvent : Enum
    {
        public event Action<IState<TEvent>, IState<TEvent>> onStateChange;

        public abstract IState<TEvent> initState { get; }
        public IState<TEvent> currentState { get; private set; }

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

            currentState?.Exit();

            onStateChange?.Invoke(currentState, state);

            currentState = state;

            if (currentState != null)
            {
                currentState.FSM = this;
                currentState.Enter();
            }
        }
    }
}
