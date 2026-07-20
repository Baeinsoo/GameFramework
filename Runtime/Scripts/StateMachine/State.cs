using System;
using System.Threading;
using System.Threading.Tasks;

namespace GameFramework
{
    public abstract class State<TEvent> : IState<TEvent> where TEvent : struct, Enum
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
                Fire(await OnExecuteAsync(ct), ct);
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

                Fire(OnError(e), ct);
            }
        }

        //  상태가 반환한 이벤트를 대신 발행. 취소됐으면(이미 다른 상태) 발행하지 않는다.
        //  이 취소 체크가 한 곳에 있어, 상태는 Fire도 취소 확인도 직접 하지 않는다.
        private void Fire(TEvent? ev, CancellationToken ct)
        {
            if (ct.IsCancellationRequested || ev.HasValue == false)
            {
                return;
            }

            FSM.Fire(ev.Value);
        }

        public abstract IState<TEvent> GetNextState(TEvent ev);

        protected virtual void OnEnter() { }
        protected virtual void OnExit() { }

        //  상태의 비동기 작업. 다음 이벤트를 반환하면 base가 발행한다(직접 FSM.Fire 하지 않음).
        //  전이할 이벤트가 없으면 null.
        protected virtual Task<TEvent?> OnExecuteAsync(CancellationToken ct) => Task.FromResult<TEvent?>(null);

        //  미처리 예외 시 회복 이벤트를 반환(없으면 null). 발행은 base가 한다.
        protected virtual TEvent? OnError(Exception e) => null;
    }
}
