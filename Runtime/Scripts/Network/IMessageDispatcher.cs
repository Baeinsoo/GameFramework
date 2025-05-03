using System;
using UnityEngine;

namespace GameFramework
{
    public interface IMessageDispatcher
    {
        void RegisterHandler<T>(Action<T> handler) where T : IMessage;
        void UnregisterHandler<T>(Action<T> handler) where T : IMessage;
        void EnqueueMessage(IMessage message);
    }
}
