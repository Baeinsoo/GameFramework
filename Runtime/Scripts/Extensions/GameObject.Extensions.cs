using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public static partial class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            return self.GetComponent<T>() ?? self.AddComponent<T>();
        }
    }
}
