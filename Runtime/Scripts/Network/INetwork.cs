using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface INetwork<TMessage> where TMessage : IMessage
    {
        event Action<int, TMessage> onMessage;

        void Send(TMessage message, int targetId, bool reliable = true, bool instant = false);
        void SendToAll(TMessage message, bool reliable = true, bool instant = false);
        void SendToNear(TMessage message, Vector3 center, float radius, bool reliable = true, bool instant = false);
    }
}
