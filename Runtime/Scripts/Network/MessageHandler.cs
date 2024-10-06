using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class MessageHandlerBase
    {
        public abstract void Invoke(IMessage message);
        public abstract bool IsEmpty { get; }
    }

    public class MessageHandler<T> : MessageHandlerBase where T : IMessage
    {
        private Action<T> handlers;

        public override void Invoke(IMessage message)
        {
            handlers?.Invoke((T)message);
        }

        public void AddHandler(Action<T> handler)
        {
            handlers += handler;
        }

        public void RemoveHandler(Action<T> handler)
        {
            handlers -= handler;
        }

        public override bool IsEmpty => handlers == null;
    }
}
