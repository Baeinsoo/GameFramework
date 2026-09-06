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

        /// <summary>
        /// 틱 시스템이 던진 예외로 루프가 끝났나. 조용히 죽던 것을 드러내려고 남긴다 —
        /// 이 값이 참이면 시뮬은 <b>더 이상 돌지 않는다</b>.
        /// </summary>
        bool isFaulted { get; }

        /// <summary>루프가 끝난 틱. <see cref="isFaulted"/>가 참일 때만 의미가 있다.</summary>
        long faultedTick { get; }

        void Run(long tick, double interval, double elapsedTime);
        void Stop();
    }
}
