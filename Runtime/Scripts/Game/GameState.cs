using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public enum GameState
    {
        None = 0,
        Initializing = 1,
        Preparing = 2,
        Playing = 3,
        Paused = 4,
        GameOver = 5,
    }
}
