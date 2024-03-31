using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework
{
    public interface ITickUpdater
    {
        event Action<long> onTick;

        long tick { get; }
        double interval { get; }
        double elapsedTime { get; }
        long processibleTick { get; }

        void Run(long tick, double interval, double elapsedTime);
        void Stop();
    }
}
