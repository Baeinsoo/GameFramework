using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace GameFramework.Tests
{
    public class StateMachineTests
    {
        private enum E { Go, Go2, Recover }

        // 테스트용 상태: 전이표·비동기 작업·에러 회복을 델리게이트로 주입한다.
        private sealed class TestState : State<E>
        {
            public string name;
            public Func<CancellationToken, Task<E?>> onExec;
            public Func<Exception, E?> onErr;
            public readonly Dictionary<E, IState<E>> transitions = new Dictionary<E, IState<E>>();

            protected override Task<E?> OnExecuteAsync(CancellationToken ct)
                => onExec != null ? onExec(ct) : Task.FromResult<E?>(null);

            protected override E? OnError(Exception e)
                => onErr != null ? onErr(e) : (E?)null;

            public override IState<E> GetNextState(E ev)
                => transitions.TryGetValue(ev, out var next) ? next : this;
        }

        private sealed class TestMachine : StateMachine<E>
        {
            private readonly IState<E> init;
            public TestMachine(IState<E> init) { this.init = init; }
            public override IState<E> initState => init;
        }

        [Test]
        public void Start_EntersInitState()
        {
            var a = new TestState { name = "A" };
            var m = new TestMachine(a);

            m.Start();

            Assert.AreSame(a, m.currentState);
        }

        [Test]
        public void Fire_TransitionsPerGetNextState()
        {
            var a = new TestState { name = "A" };
            var b = new TestState { name = "B" };
            a.transitions[E.Go] = b;
            var m = new TestMachine(a);
            m.Start();

            m.Fire(E.Go);

            Assert.AreSame(b, m.currentState);
        }

        [Test]
        public void Fire_UnhandledEvent_StaysInState()
        {
            var a = new TestState { name = "A" };   // 전이표 비어 있음 → GetNextState가 자기 자신 반환 = 무시
            var m = new TestMachine(a);
            m.Start();

            m.Fire(E.Go);

            Assert.AreSame(a, m.currentState);
        }

        [Test]
        public void OnExecuteAsync_ReturnedEvent_Transitions()
        {
            var a = new TestState { name = "A" };
            var b = new TestState { name = "B" };
            a.onExec = ct => Task.FromResult<E?>(E.Go);   // 진입하자마자 Go 반환
            a.transitions[E.Go] = b;
            var m = new TestMachine(a);

            m.Start();   // A.Enter가 OnExecuteAsync를 동기 완료로 돌려 base가 Go를 발행 → B

            Assert.AreSame(b, m.currentState);
        }

        [Test]
        public void OnExecuteAsync_ReturnsNull_NoTransition()
        {
            var a = new TestState { name = "A" };
            a.onExec = ct => Task.FromResult<E?>(null);   // 반환 이벤트 없음 → 발행 안 함
            var m = new TestMachine(a);

            m.Start();

            Assert.AreSame(a, m.currentState);
        }

        [Test]
        public void OnError_ReturnedEvent_Transitions()
        {
            var a = new TestState { name = "A" };
            var b = new TestState { name = "B" };
            a.onExec = ct => throw new InvalidOperationException("boom");   // 미처리 예외
            a.onErr = e => E.Recover;                                      // 회복 이벤트
            a.transitions[E.Recover] = b;
            var m = new TestMachine(a);

            m.Start();   // 예외 → OnError가 Recover 반환 → base가 발행 → B

            Assert.AreSame(b, m.currentState);
        }

        // 전이 도중(onStateChange 콜백) 다시 Fire되면, 현재 전이를 끝낸 뒤 큐에서 처리해야 한다
        // (UML statechart의 run-to-completion). 재귀 전이가 아니라 평탄한 순서여야 한다.
        [Test]
        public void ReentrantFire_DuringTransition_ProcessedAfterCurrent()
        {
            var a = new TestState { name = "A" };
            var b = new TestState { name = "B" };
            var c = new TestState { name = "C" };
            a.onExec = ct => Task.FromResult<E?>(E.Go);   // Fire 루프 안에서 A→B를 구동
            a.transitions[E.Go] = b;
            b.transitions[E.Go2] = c;

            var log = new List<string>();
            var m = new TestMachine(a);
            bool fired = false;
            m.onStateChange += (from, to) =>
            {
                string n = (to as TestState)?.name ?? "null";
                log.Add("in:" + n);
                if (to == b && fired == false)
                {
                    fired = true;
                    m.Fire(E.Go2);   // 재진입 — 큐에 쌓여 B 전이 완료 후 처리돼야 함
                }
                log.Add("out:" + n);
            };

            m.Start();

            Assert.AreSame(c, m.currentState);
            CollectionAssert.AreEqual(
                new[] { "in:A", "out:A", "in:B", "out:B", "in:C", "out:C" },
                log);
        }
    }
}
