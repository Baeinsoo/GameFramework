using System;
using NUnit.Framework;

namespace GameFramework.Tests
{
    public class MessageHandlerBaseTests
    {
        private sealed class FakeDisposable : IDisposable
        {
            public bool Disposed { get; private set; }
            public void Dispose() => Disposed = true;
        }

        private sealed class TestHandler : MessageHandlerBase
        {
            public bool SubscribeCalled { get; private set; }
            public readonly FakeDisposable Sub = new FakeDisposable();

            protected override void Subscribe()
            {
                SubscribeCalled = true;
                Track(Sub);
            }
        }

        [Test]
        public void Initialize_calls_Subscribe()
        {
            var handler = new TestHandler();
            ((VContainer.Unity.IInitializable)handler).Initialize();
            Assert.IsTrue(handler.SubscribeCalled);
        }

        [Test]
        public void Dispose_disposes_tracked_subscriptions()
        {
            var handler = new TestHandler();
            ((VContainer.Unity.IInitializable)handler).Initialize();
            Assert.IsFalse(handler.Sub.Disposed);

            handler.Dispose();
            Assert.IsTrue(handler.Sub.Disposed);
        }
    }
}
