using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public static partial class Extensions
    {
        public static T GetOrAddComponent<T>(this MonoBehaviour self) where T : Component
        {
            return self.gameObject.GetComponent<T>() ?? self.gameObject.AddComponent<T>();
        }
    }
}
