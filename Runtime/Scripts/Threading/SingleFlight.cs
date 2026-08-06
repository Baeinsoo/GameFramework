using System;
using Cysharp.Threading.Tasks;

namespace GameFramework.Threading
{
    /// <summary>같은 작업이 이미 돌고 있으면 새로 시작하지 않고 그 결과를 함께 기다린다.
    /// 동시에 들어온 호출 N개를 실제 실행 1번으로 접는다.</summary>
    /// <remarks>Unity 메인 스레드 전용이라 락이 없다.</remarks>
    public class SingleFlight<T>
    {
        private bool inFlight;
        private UniTask<T> pending;

        public UniTask<T> RunAsync(Func<UniTask<T>> operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (inFlight)
            {
                return pending;
            }

            inFlight = true;

            //  Preserve가 없으면 두 번째 대기자가 터진다 — UniTask는 기본적으로 한 번만 await할 수 있다.
            pending = RunAndReleaseAsync(operation).Preserve();
            return pending;
        }

        private async UniTask<T> RunAndReleaseAsync(Func<UniTask<T>> operation)
        {
            try
            {
                return await operation.Invoke();
            }
            finally
            {
                //  성공이든 실패든 자리를 비운다. 결과를 캐시하지 않으므로 다음 호출은 새로 실행된다.
                inFlight = false;
            }
        }
    }
}
