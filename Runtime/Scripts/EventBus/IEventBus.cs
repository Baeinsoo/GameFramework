using System;

namespace GameFramework
{
    public interface IEventBus
    {
        void Subscribe<T>(string topic, Action<T> handler);

        void Unsubscribe<T>(string topic, Action<T> handler);
        void UnsubscribeAll(string topic);

        void Publish<T>(string topic, T data);

        void Clear();

        bool HasTopic(string topic);
        int GetHandlerCount(string topic);
    }
}
