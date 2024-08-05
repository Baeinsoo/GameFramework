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

        public static void Set<T>(T data)
        {
            dataMap[typeof(T).Name] = data;
        }

        public static void Set<T>(string key, T data)
        {
            dataMap[key] = data;
        }

        public static T Get<T>(string key = null, bool delete = false)
        {
            try
            {
                return (T)dataMap[key ?? typeof(T).Name];
            }
            finally
            {
                if (delete)
                {
                    Delete<T>(key);
                }
            }
        }

        public static void Delete<T>(string key = null)
        {
            dataMap.Remove(key ?? typeof(T).Name);
        }
    }
}
