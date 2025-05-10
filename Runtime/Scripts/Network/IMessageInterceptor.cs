using System;

namespace GameFramework
{
    public interface IMessageInterceptor
    {
        void OnBeforeHandle<T>(T message) where T : IMessage;
        void OnAfterHandle<T>(T message) where T : IMessage;
        void OnError<T>(T message, Exception e) where T : IMessage;
    }
}
