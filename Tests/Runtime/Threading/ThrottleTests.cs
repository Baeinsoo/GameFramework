using System;
using GameFramework.Threading;
using NUnit.Framework;

namespace GameFramework.Tests.Threading
{
    public class ThrottleTests
    {
        private static readonly DateTimeOffset Origin = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

        [Test]
        public void 처음에는_통과시킨다()
        {
            var throttle = new Throttle(TimeSpan.FromSeconds(30));

            Assert.That(throttle.TryAcquire(Origin), Is.True);
        }

        [Test]
        public void 간격_안에_다시_물으면_막는다()
        {
            var throttle = new Throttle(TimeSpan.FromSeconds(30));
            throttle.TryAcquire(Origin);

            Assert.That(throttle.TryAcquire(Origin.AddSeconds(29)), Is.False);
        }

        [Test]
        public void 간격이_지나면_다시_통과시킨다()
        {
            var throttle = new Throttle(TimeSpan.FromSeconds(30));
            throttle.TryAcquire(Origin);

            Assert.That(throttle.TryAcquire(Origin.AddSeconds(30)), Is.True);
        }

        [Test]
        public void 막힌_호출은_시각을_밀지_않는다()
        {
            //  막을 때마다 시각이 밀리면, 1초 간격으로 두드리는 재시도 루프가 창을 영원히 연장해
            //  정상 복구까지 막아버린다.
            var throttle = new Throttle(TimeSpan.FromSeconds(30));
            throttle.TryAcquire(Origin);
            throttle.TryAcquire(Origin.AddSeconds(20));

            Assert.That(throttle.TryAcquire(Origin.AddSeconds(30)), Is.True);
        }
    }
}
