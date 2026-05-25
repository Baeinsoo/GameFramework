using System;

namespace GameFramework
{
    public interface IState<TEvent> where TEvent : Enum
    {
        IStateMachine<TEvent> FSM { get; set; }

        void Enter();
        void Exit();

        IState<TEvent> GetNextState(TEvent ev);
    }
}
