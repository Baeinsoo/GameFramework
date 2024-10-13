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
    }
}
