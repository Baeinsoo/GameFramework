using NUnit.Framework;
using GameFramework.Netcode;

namespace GameFramework.Tests
{
    public class ClockSettleDetectorTests
    {
        private static ClockSettleDetector Create() =>
            new ClockSettleDetector(windowSeconds: 0.5, minSamples: 5, amplitudeThreshold: 0.005);

        [Test]
        public void IsSettled_BeforeAnySample_False()
        {
            var detector = Create();
            Assert.IsFalse(detector.IsSettled);
        }

        [Test]
        public void IsSettled_WindowNotYetSpanned_False()
        {
            var detector = Create();
            // 표본 수는 충분하지만 아직 0.5초가 안 흘렀다.
            for (int i = 0; i < 10; i++)
            {
                detector.Feed(i * 0.02, 1.0);
            }
            Assert.IsFalse(detector.IsSettled);
        }

        [Test]
        public void IsSettled_TooFewSamples_False()
        {
            var detector = Create();
            // 창은 넉넉히 지났지만 표본이 3개뿐 — 프레임이 정체된 상황.
            detector.Feed(0.0, 1.0);
            detector.Feed(0.4, 1.0);
            detector.Feed(0.8, 1.0);
            Assert.IsFalse(detector.IsSettled);
        }

        [Test]
        public void IsSettled_AmplitudeAboveThreshold_False()
        {
            var detector = Create();
            for (int i = 0; i < 10; i++)
            {
                detector.Feed(i * 0.1, i * 0.01);   // 창 안에서 계속 움직인다
            }
            Assert.IsFalse(detector.IsSettled);
        }

        [Test]
        public void IsSettled_WindowSpannedAndFlat_True()
        {
            var detector = Create();
            for (int i = 0; i < 10; i++)
            {
                detector.Feed(i * 0.1, 1.0 + i * 0.0001);   // 총 진폭 0.9ms
            }
            Assert.IsTrue(detector.IsSettled);
        }

        [Test]
        public void Feed_DropsSamplesOlderThanWindow()
        {
            var detector = Create();
            detector.Feed(0.0, 5.0);   // 창 밖으로 밀려나야 하는 큰 값
            for (int i = 1; i <= 10; i++)
            {
                detector.Feed(i * 0.1, 1.0);
            }
            // 5.0이 아직 창에 있었다면 진폭이 4.0이라 안정일 수 없다.
            Assert.IsTrue(detector.IsSettled);
            Assert.AreEqual(0.0, detector.Amplitude, 1e-9);
        }

        [Test]
        public void Amplitude_BeforeFirstEvaluation_IsNaN()
        {
            var detector = Create();
            detector.Feed(0.0, 1.0);
            Assert.IsNaN(detector.Amplitude);
        }

        [Test]
        public void Amplitude_AfterEvaluation_IsWindowRange()
        {
            var detector = Create();
            for (int i = 0; i < 10; i++)
            {
                detector.Feed(i * 0.1, i % 2 == 0 ? 1.0 : 1.02);
            }
            Assert.AreEqual(0.02, detector.Amplitude, 1e-9);
        }

        // 계약 명시: 이 판정기는 "숫자열이 멈췄나"만 본다. 값이 계속 0이면 멈춘 것이 맞다.
        // 그 0이 "아직 아무것도 측정되지 않았다"는 뜻인지는 값의 출처만 알 수 있으므로,
        // 출처가 비어 있는 동안 Feed하지 않는 것은 호출자 책임이다(예: RTT가 0이면 안 넣는다).
        // 여기서 걸러주도록 "고치면" 정상적으로 안정된 0 계열을 영영 못 받아들이게 된다.
        [Test]
        public void IsSettled_ConstantZeroSeries_True_CallerMustGateOnSourceReadiness()
        {
            var detector = Create();
            for (int i = 0; i < 10; i++)
            {
                detector.Feed(i * 0.1, 0.0);
            }
            Assert.IsTrue(detector.IsSettled);
        }

        [Test]
        public void Reset_ClearsWindowAndAmplitude()
        {
            var detector = Create();
            for (int i = 0; i < 10; i++)
            {
                detector.Feed(i * 0.1, 1.0);
            }
            Assert.IsTrue(detector.IsSettled);

            detector.Reset();

            Assert.IsFalse(detector.IsSettled);
            Assert.IsNaN(detector.Amplitude);
            Assert.AreEqual(0, detector.SampleCount);
        }

        [Test]
        public void SampleCount_ReflectsSamplesInsideWindow()
        {
            var detector = Create();
            for (int i = 0; i < 10; i++)
            {
                detector.Feed(i * 0.1, 1.0);
            }
            // 0.0~0.9초 중 창(0.5초) 안에 남는 것은 0.4초 이후 표본들.
            Assert.AreEqual(6, detector.SampleCount);
        }
    }
}
