using System;
using UnityEngine;

namespace GameFramework
{
    public interface IEventBus
    {
        void Publish<T>(T eventData);
        void Subscribe<T>(Action<T> handler);
        void Unsubscribe<T>(Action<T> handler);
    }
}
