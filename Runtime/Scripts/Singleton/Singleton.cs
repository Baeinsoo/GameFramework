using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class Singleton<T> where T : class, new()
    {
        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    Instantiate();
                }

                return _instance;
            }
        }

        private static void Instantiate()
        {
            _instance = new T();
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }
    }
}
