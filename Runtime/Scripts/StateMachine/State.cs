using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework
{
    public abstract class State<TEvent> : IState<TEvent> where TEvent : Enum
    {
        public IStateMachine<TEvent> FSM { get; set; }

        private CancellationTokenSource cts;

        public void Enter()
        {
            cts = new CancellationTokenSource();
            OnEnter();
            _ = ExecuteAsync(cts.Token);
        }

        public void Exit()
        {
            cts?.Cancel();
            OnExit();
            cts?.Dispose();
            cts = null;
        }

        private async Task ExecuteAsync(CancellationToken ct)
        {
            try
            {
                await OnExecuteAsync(ct);
            }
            catch (OperationCanceledException)
            {
                //  State exited; expected on cancellation.
            }
        }

        public abstract IState<TEvent> GetNextState(TEvent ev);

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual Task OnExecuteAsync(CancellationToken ct) => Task.CompletedTask;
    }
}
