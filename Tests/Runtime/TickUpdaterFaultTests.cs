using System;
using GameFramework.Runner;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GameFramework.Tests
{
    /// <summary>
    /// 틱 시스템이 예외를 던지면 루프가 <b>멈추고, 멈췄다는 사실이 남는지</b>를 지킨다.
    ///
    /// <para>이 보증이 없을 때 실제로 이랬다(2026-09-06): 입력 시스템이 없는 엔티티를 읽어 NRE를
    /// 냈고 코루틴이 죽어 클라 시뮬 전체가 멈췄는데, 콘솔에는 그 NRE 한 줄뿐이었다. 남의 캐릭터는
    /// 다른 경로로 계속 움직여 정상처럼 보였고, 원인을 찾는 데 로그를 훑어야 했다.</para>
    ///
    /// <para>코루틴은 EditMode에서 못 돌리므로 상속으로 한 틱만 직접 부른다.</para>
    /// </summary>
    public class TickUpdaterFaultTests
    {
        private sealed class Probe : TickUpdaterBase
        {
            public bool RunOneTick() => TryTickBody();
        }

        private GameObject host;

        private Probe NewProbe()
        {
            host = new GameObject(nameof(Probe));
            return host.AddComponent<Probe>();
        }

        [TearDown]
        public void TearDown()
        {
            if (host != null)
            {
                UnityEngine.Object.DestroyImmediate(host);
                host = null;
            }
        }

        [Test]
        public void 정상이면_틱이_늘고_멈춤_표시가_없다()
        {
            var probe = NewProbe();
            int calls = 0;
            probe.onTick += _ => calls++;

            Assert.IsTrue(probe.RunOneTick());
            Assert.IsTrue(probe.RunOneTick());

            Assert.AreEqual(2, calls);
            Assert.AreEqual(2, probe.tick);
            Assert.IsFalse(probe.isFaulted);
        }

        [Test]
        public void 틱_시스템이_던지면_루프를_끝내라고_알린다()
        {
            var probe = NewProbe();
            probe.onTick += _ => throw new InvalidOperationException("일부러");

            LogAssert.Expect(LogType.Exception, "InvalidOperationException: 일부러");
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("멈췄다"));

            Assert.IsFalse(probe.RunOneTick(), "루프를 계속 돌리라고 답하면 안 된다");
        }

        [Test]
        public void 던지면_멈춘_사실과_그_틱이_남는다()
        {
            var probe = NewProbe();
            bool explode = false;
            probe.onTick += _ =>
            {
                if (explode)
                {
                    throw new InvalidOperationException("일부러");
                }
            };

            probe.RunOneTick();   // tick 0 정상
            probe.RunOneTick();   // tick 1 정상

            explode = true;
            LogAssert.Expect(LogType.Exception, "InvalidOperationException: 일부러");
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("멈췄다"));
            probe.RunOneTick();   // tick 2 에서 터진다

            Assert.IsTrue(probe.isFaulted);
            Assert.AreEqual(2, probe.faultedTick, "터진 그 틱이어야 한다");
            Assert.AreEqual(2, probe.tick, "터진 틱은 증가하지 않는다 — 그 틱은 끝나지 않았다");
        }

        [Test]
        public void 다시_돌리면_멈춤_표시가_지워진다()
        {
            var probe = NewProbe();
            Action<long> boom = _ => throw new InvalidOperationException("일부러");
            probe.onTick += boom;

            LogAssert.Expect(LogType.Exception, "InvalidOperationException: 일부러");
            LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex("멈췄다"));
            probe.RunOneTick();
            Assert.IsTrue(probe.isFaulted);

            //  터뜨리던 구독자를 뗀 뒤 다시 돌린다. 안 떼면 Run이 코루틴 본문을 첫 yield까지
            //  그 자리에서 실행하면서 곧바로 또 터져, 초기화가 됐는지 알 수 없다.
            probe.onTick -= boom;
            probe.Run(0, 0.02, 0);
            probe.Stop();

            Assert.IsFalse(probe.isFaulted, "새 에피소드는 지난 멈춤을 물려받지 않는다");
            Assert.AreEqual(0, probe.faultedTick);
        }
    }
}
