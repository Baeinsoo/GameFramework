using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class GameEngineListenAttribute : Attribute
    {
        public Type type { get; }

        public GameEngineListenAttribute(Type type)
        {
            this.type = type;
        }
    }
}
