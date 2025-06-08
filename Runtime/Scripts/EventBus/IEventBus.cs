using System;

namespace GameFramework
{
    public interface IEventBus
    {
        void Publish<T>(T eventData);
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
        void Clear();
    }
}
