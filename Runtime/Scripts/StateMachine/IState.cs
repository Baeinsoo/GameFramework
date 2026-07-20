using System;

namespace GameFramework
{
    public interface IState<TEvent> where TEvent : struct, Enum
    {
        IStateMachine<TEvent> FSM { get; set; }

        void Enter();
        void Exit();

        IState<TEvent> GetNextState(TEvent ev);
    }
}
