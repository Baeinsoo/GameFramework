using System;

namespace GameFramework
{
    public interface IStateMachine<TEvent> where TEvent : struct, Enum
    {
        event Action<IState<TEvent>, IState<TEvent>> onStateChange;

        IState<TEvent> initState { get; }
        IState<TEvent> currentState { get; }

        void Fire(TEvent ev);
    }
}
