using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface INetwork
    {
        event Action<int, IMessage> onMessage;

        void Send(IMessage message, int targetId, bool reliable = true, bool instant = false);
        void SendToAll(IMessage message, bool reliable = true, bool instant = false);
        void SendToNear(IMessage message, Vector3 center, float radius, bool reliable = true, bool instant = false);
    }
}
