using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class DataStorage : MonoBehaviour
    {
        private Dictionary<string, object> keyDataMap;
        private Dictionary<Type, object> typeDataMap;

        private void Awake()
        {
            keyDataMap = new Dictionary<string, object>();
            typeDataMap = new Dictionary<Type, object>();
        }

        private void OnDestroy()
        {
            keyDataMap.Clear();
            keyDataMap = null;

            typeDataMap.Clear();
            typeDataMap = null;
        }

        public void Set<T>(T value)
        {
            typeDataMap[typeof(T)] = value;
        }

        public void Set<T>(string key, T value)
        {
            keyDataMap[key] = value;
        }

        public bool Get<T>(out T value, bool delete = false)
        {
            try
            {
                if (typeDataMap.TryGetValue(typeof(T), out var data))
                {
                    value = (T)data;
                    return true;
                }
                else
                {
                    value = default(T);
                    return false;
                }
            }
            finally
            {
                if (delete)
                {
                    if (typeDataMap.ContainsKey(typeof(T)))
                    {
                        typeDataMap.Remove(typeof(T));
                    }
                }
            }

        }

        public bool Get<T>(string key, out T value, bool delete = false)
        {
            try
            {
                if (keyDataMap.TryGetValue(key, out var data))
                {
                    value = (T)data;
                    return true;
                }
                else
                {
                    value = default(T);
                    return false;
                }
            }
            finally
            {
                if (delete)
                {
                    if (keyDataMap.ContainsKey(key))
                    {
                        keyDataMap.Remove(key);
                    }
                }
            }
        }
    }
}
