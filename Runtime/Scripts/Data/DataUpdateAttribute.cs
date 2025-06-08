using System;

namespace GameFramework
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DataListenAttribute : Attribute
    {
        public Type ListenType { get; }

        public DataListenAttribute(Type listenType)
        {
            ListenType = listenType;
        }
    }
}
