using System;

namespace GameFramework
{
    public interface IEventBus
    {
        void Subscribe<T>(string topic, Action<T> handler);
        void Subscribe<T>(string topic, Action<T> handler, int priority);

        void Unsubscribe<T>(string topic, Action<T> handler);
        void UnsubscribeAll(string topic);

        void Publish<T>(string topic, T data);

        void Clear();
    }
}
