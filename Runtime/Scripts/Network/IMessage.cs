using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IMessage
    {
        ushort messageId { get; }

        byte[] Serialize();
        void Deserialize(byte[] data);
    }
}
