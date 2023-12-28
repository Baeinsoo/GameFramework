using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class Game
    {
        public static IGame current { get; internal set; }

        public class Time
        {
            public static long tick => Game.current.tickUpdater.tick;
            public static double time => Game.current.tickUpdater.time;
            public static double elapsedTime => Game.current.tickUpdater.elapsedTime;

            /// <summary> Tick Delta Time </summary>
            public static double deltaTime => Game.current.tickUpdater.tick == 0 ? 0 : Game.current.tickUpdater.interval;
        }
    }
}
