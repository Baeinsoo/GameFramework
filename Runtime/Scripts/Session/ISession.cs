using System;

namespace GameFramework
{
    public interface ISession : IDisposable
    {
        string sessionId { get; }
        string userId { get; }

        bool isConnected { get; }

        // reliable=false면 unreliable 채널로(시간민감 입력용). 채널 매핑은 구현체(LOPSession) 책임 — GameFramework는 Mirror 비의존.
        void Send<T>(T message, bool reliable = true) where T : IMessage;
        IMessage Receive();
    }
}
