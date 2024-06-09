using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramework
{
    public class ScriptableObjectSingleton<T> : ScriptableObject where T : ScriptableObjectSingleton<T>
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    var candidates = Resources.LoadAll<T>("");
                    if (candidates == null || candidates.Length < 1)
                    {
                        throw new Exception($"Could not find any ScriptableObjectSingleton instance in the Resources.");
                    }
                    else if (candidates.Length > 1)
                    {
                        Debug.LogWarning($"Multiple instances of the ScriptableObjectSingleton found in the Resources.");
                    }

                    _instance = Instantiate(candidates[0]);
                }

                return _instance;
            }
        }
    }
}
