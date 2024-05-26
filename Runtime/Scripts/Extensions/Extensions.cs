using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public static partial class Extensions
    {
        public static bool TryParse<T>(this Enum self, out T result) where T : struct
        {
            return Enum.TryParse(self.ToString(), out result);
        }

        public static T Parse<T>(this string self)
        {
            return (T)Enum.Parse(typeof(T), self);
        }
    }
}
