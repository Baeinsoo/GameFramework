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
                //  상태를 빠져나가며 취소됨 — 정상.
            }
            catch (Exception e)
            {
                //  이미 다른 상태로 넘어간 뒤 뒤늦게 터진 예외는 무시.
                if (ct.IsCancellationRequested)
                {
                    return;
                }

                OnError(e);
            }
        }

        public abstract IState<TEvent> GetNextState(TEvent ev);

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }
        protected virtual Task OnExecuteAsync(CancellationToken ct) => Task.CompletedTask;

        //  OnExecuteAsync에서 처리 못한 예외가 났을 때 호출. 어디로 갈지는 상태가 정한다.
        protected virtual void OnError(Exception e) { }
    }
}
