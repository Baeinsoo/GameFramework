using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GameFramework.Threading;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GameFramework.Tests.Threading
{
    public class SingleFlightTests
    {
        [UnityTest]
        public IEnumerator 동시에_들어온_호출은_한_번만_실행된다() => UniTask.ToCoroutine(async () =>
        {
            var flight = new SingleFlight<int>();
            var gate = new UniTaskCompletionSource<int>();
            int executions = 0;

            UniTask<int> Operation()
            {
                executions++;
                return gate.Task;
            }

            UniTask<int> first = flight.RunAsync(Operation);
            UniTask<int> second = flight.RunAsync(Operation);
            UniTask<int> third = flight.RunAsync(Operation);

            gate.TrySetResult(42);
            //  UniTask.WhenAll(a,b,c)는 튜플 오버로드가 우선 선택돼 (int,int,int)를 반환한다 —
            //  int[]를 받으려면 단일 타입 파라미터 오버로드로 명시해야 한다.
            int[] results = await UniTask.WhenAll<int>(first, second, third);

            Assert.That(executions, Is.EqualTo(1));
            Assert.That(results, Is.EqualTo(new[] { 42, 42, 42 }));
        });

        [UnityTest]
        public IEnumerator 끝난_뒤에_부르면_다시_실행된다() => UniTask.ToCoroutine(async () =>
        {
            //  결과를 캐시해 버리면 토큰이 만료돼도 영영 갱신되지 않는다.
            var flight = new SingleFlight<int>();
            int executions = 0;

            UniTask<int> Operation()
            {
                executions++;
                return UniTask.FromResult(executions);
            }

            await flight.RunAsync(Operation);
            await flight.RunAsync(Operation);

            Assert.That(executions, Is.EqualTo(2));
        });

        [UnityTest]
        public IEnumerator 실패는_모든_대기자에게_전달된다() => UniTask.ToCoroutine(async () =>
        {
            var flight = new SingleFlight<int>();
            var gate = new UniTaskCompletionSource<int>();

            UniTask<int> first = flight.RunAsync(() => gate.Task);
            UniTask<int> second = flight.RunAsync(() => gate.Task);

            gate.TrySetException(new InvalidOperationException("갱신 실패"));

            Assert.That(await CatchAsync(first), Is.InstanceOf<InvalidOperationException>());
            Assert.That(await CatchAsync(second), Is.InstanceOf<InvalidOperationException>());
        });

        [UnityTest]
        public IEnumerator 실패한_뒤에_부르면_다시_시도한다() => UniTask.ToCoroutine(async () =>
        {
            //  실패를 캐시하면 네트워크가 한 번 끊긴 뒤로 영영 갱신을 시도하지 않게 된다.
            var flight = new SingleFlight<int>();
            int executions = 0;

            UniTask<int> Failing()
            {
                executions++;
                return UniTask.FromException<int>(new InvalidOperationException("갱신 실패"));
            }

            await CatchAsync(flight.RunAsync(Failing));
            await CatchAsync(flight.RunAsync(Failing));

            Assert.That(executions, Is.EqualTo(2));
        });

        //  NUnit의 Throws 제약은 UniTask를 다루지 못해서 직접 잡는다.
        private static async UniTask<Exception> CatchAsync(UniTask<int> task)
        {
            try
            {
                await task;
                return null;
            }
            catch (Exception exception)
            {
                return exception;
            }
        }
    }
}
