
namespace GameFramework
{
    public interface IMessageInterceptor
    {
        void OnBeforeHandle<T>(T message) where T : IMessage;
        void OnAfterHandle<T>(T message) where T : IMessage;
        void OnError<T>(T message, string error) where T : IMessage;
    }

    public interface IMessageInterceptorWithId
    {
        void OnBeforeHandle<T>(int id, T message) where T : IMessage;
        void OnAfterHandle<T>(int id, T message) where T : IMessage;
        void OnError<T>(int id, T message, string error) where T : IMessage;
    }
}
