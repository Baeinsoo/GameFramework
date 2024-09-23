using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GameFramework
{
    public static partial class Extensions
    {
        public static IEnumerator AsIEnumerator(this Task self)
        {
            while (!self.IsCompleted)
            {
                yield return null;
            }

            if (self.IsFaulted)
            {
                throw self.Exception;
            }
        }
    }
}
