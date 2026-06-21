using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public static class GameEngine
    {
        public static IGameEngine current { get; internal set; }

        public static class Time
        {
            public static long tick => GameEngine.current.tickUpdater.tick;

            public static double tickInterval => GameEngine.current.tickUpdater.interval;

            /// <summary> tickTime (tick * interval) </summary>
            public static double tickTime => GameEngine.current.tickUpdater.tick * GameEngine.current.tickUpdater.interval;

            /// <summary> real elapsed time since the game started </summary>
            public static double elapsedTime => GameEngine.current.tickUpdater.elapsedTime;

            /// <summary> Tick Delta Time </summary>
            public static double deltaTime => GameEngine.current.tickUpdater.tick == 0 ? 0 : GameEngine.current.tickUpdater.interval;
        }

        /// <summary>
        /// 네트워크 동기 시간 facade. 양쪽 사용 — 서버는 <see cref="serverNow"/>(권위 시간), 클라는 <see cref="predictedTime"/>/<see cref="rtt"/>.
        /// </summary>
        public static class NetworkTime
        {
            /// <summary> 현재 서버 시간(추정). 서버=권위, 클라=predictedTime − RTT/2. </summary>
            public static double serverNow => GameEngine.current.networkTime.ServerNow;

            public static double predictedTime => GameEngine.current.networkTime.PredictedTime;

            public static double rtt => GameEngine.current.networkTime.Rtt;

            /// <summary> 원격 보간 back-time = RTT/2 (소비자가 clamp). </summary>
            public static double remoteBackTime => GameEngine.current.networkTime.RemoteBackTime();
        }
    }
}
