using System;
using System.Collections;
using System.Collections.Generic;

namespace GameFramework.Runner
{
    public interface ITickUpdater
    {
        event Action<long> onTick;

        long tick { get; }
        double interval { get; }
        double elapsedTime { get; }
        long processibleTick { get; }
        double deltaTime { get; }

        /// <summary>캐치업 상한에 걸린 횟수(에피소드 단위). 측정 창에 멈춤이 있었는지 가리는 데 쓴다.</summary>
        int catchUpCappedCount { get; }

        /// <summary>상한에 걸린 동안 관측된 최대 뒤처짐(틱) — 멈춤의 크기.</summary>
        long maxTicksBehind { get; }

        void Run(long tick, double interval, double elapsedTime);
        void Stop();
    }
}
