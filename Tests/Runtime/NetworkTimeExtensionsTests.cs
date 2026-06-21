using NUnit.Framework;

namespace GameFramework.Tests
{
    public class NetworkTimeExtensionsTests
    {
        private class FakeNetworkTime : INetworkTime
        {
            public double ServerNow { get; set; }
            public double PredictedTime { get; set; }
            public double Rtt { get; set; }
        }

        [Test]
        public void RemoteBackTime_IsHalfRtt()
        {
            var networkTime = new FakeNetworkTime { ServerNow = 9.9, PredictedTime = 10.0, Rtt = 0.2 };
            Assert.AreEqual(0.1, networkTime.RemoteBackTime(), 1e-9);
        }
    }
}
