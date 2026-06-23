using System;
using UnityEngine;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RunnerListenAttribute : Attribute
    {
        public Type type { get; }

        public RunnerListenAttribute(Type type)
        {
            this.type = type;
        }
    }
}
