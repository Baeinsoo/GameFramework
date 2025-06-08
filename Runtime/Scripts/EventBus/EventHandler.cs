using System;
using UnityEngine;

namespace GameFramework
{
    public abstract class EventHandlerBase
    {
        public abstract Type EventType { get; }
        public abstract bool IsEmpty { get; }

        public abstract void Invoke(object eventData);
    }

    public class EventHandler<T> : EventHandlerBase
    {
        public override Type EventType => typeof(T);
        public override bool IsEmpty => handlers == null || handlers.GetInvocationList().Length == 0;

        private Action<T> handlers;

        public override void Invoke(object eventData)
        {
            if (eventData is T typedEvent)
            {
                handlers?.Invoke(typedEvent);
            }
            else
            {
                Debug.LogError($"[EventHandler] Invalid event type: Expected {typeof(T).Name}, but got {eventData?.GetType()?.Name ?? "null"}");
            }
        }

        public void AddHandler(Action<T> handler)
        {
            handlers += handler ?? throw new ArgumentNullException(typeof(T).Name);
        }

        public void RemoveHandler(Action<T> handler)
        {
            handlers -= handler ?? throw new ArgumentNullException(typeof(T).Name);
        }
    }
}
