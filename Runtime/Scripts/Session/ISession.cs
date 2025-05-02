using System;

namespace GameFramework
{
    public interface ISession : IDisposable
    {
        string sessionId { get; }
        string userId { get; }

        bool isConnected { get; }

        void Send<T>(T message) where T : IMessage;
        IMessage Receive();
    }
}
