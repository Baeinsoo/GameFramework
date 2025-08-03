using System;

namespace GameFramework
{
    public class HandlerWrapper<T>
    {
        public Action<T> Handler { get; }

        public HandlerWrapper(Action<T> handler)
        {
            Handler = handler;
        }
    }
}
