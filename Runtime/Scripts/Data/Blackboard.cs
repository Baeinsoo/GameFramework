using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public static class Blackboard
    {
        private static Dictionary<string, object> dataMap;
        
        static Blackboard()
        {
            dataMap = new Dictionary<string, object>();
        }

        public static void Write<T>(T data)
        {
            dataMap[typeof(T).Name] = data;
        }

        public static void Write<T>(string key, T data)
        {
            dataMap[key] = data;
        }

        public static T Read<T>(string key = null, bool erase = false)
        {
            try
            {
                return (T)dataMap[key ?? typeof(T).Name];
            }
            finally
            {
                if (erase)
                {
                    Erase<T>(key);
                }
            }
        }

        public static void Erase<T>(string key = null)
        {
            dataMap.Remove(key ?? typeof(T).Name);
        }
    }
}
