using System;
using Cysharp.Threading.Tasks;

namespace GameFramework.Threading
{
    /// <summary>같은 작업이 이미 돌고 있으면 새로 시작하지 않고 그 결과를 함께 기다린다.
    /// 동시에 들어온 호출 N개를 실제 실행 1번으로 접는다.</summary>
    /// <remarks>Unity 메인 스레드 전용이라 락이 없다.</remarks>
    public class SingleFlight<T>
    {
        private UniTask<T>? pending;

        public UniTask<T> RunAsync(Func<UniTask<T>> operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (pending.HasValue)
            {
                return pending.Value;
            }

            var completion = new UniTaskCompletionSource<T>();

            //  기다릴 자리를 operation보다 먼저 게시한다 — operation은 첫 await에 닿기 전까지
            //  동기로 도는데, 그 틈에 들어온 호출이 자리를 못 찾으면 빈 태스크를 받아 조용히
            //  null을 들고 진행한다.
            //  Preserve가 없으면 두 번째 대기자가 터진다(UniTask는 기본적으로 한 번만 await 가능).
            pending = completion.Task.Preserve();

            //  이 시점의 값을 지역변수로 붙잡아 둔다 — operation이 동기로 바로 끝나버리면(예:
            //  UniTask.FromResult) 아래 호출 안에서 pending 필드가 이미 null로 비워진 뒤일 수 있어,
            //  Forget() 이후에 필드를 다시 읽으면 값 없는 Nullable을 건드리게 된다.
            UniTask<T> result = pending.Value;

            RunAndCompleteAsync(operation, completion).Forget();

            return result;
        }

        private async UniTaskVoid RunAndCompleteAsync(Func<UniTask<T>> operation, UniTaskCompletionSource<T> completion)
        {
            try
            {
                T result = await operation.Invoke();

                //  자리를 먼저 비운다 — 완료 통지를 받은 대기자가 곧바로 다시 부르면 그건 새 비행이어야 한다.
                pending = null;
                completion.TrySetResult(result);
            }
            catch (Exception exception)
            {
                pending = null;
                completion.TrySetException(exception);
            }
        }
    }
}
