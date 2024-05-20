using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class DataContainer : MonoBehaviour, IDataContainer
    {
        private Dictionary<string, object> keyToDataMap;
        private Dictionary<Type, object> typeToDataMap;

        private void Awake()
        {
            keyToDataMap = new Dictionary<string, object>();
            typeToDataMap = new Dictionary<Type, object>();
        }

        private void OnDestroy()
        {
            keyToDataMap.Clear();
            keyToDataMap = null;

            typeToDataMap.Clear();
            typeToDataMap = null;
        }

        public void Set<T>(T value)
        {
            typeToDataMap[typeof(T)] = value;
        }

        public void Set<T>(string key, T value)
        {
            keyToDataMap[key] = value;
        }

        public bool Get<T>(out T value, bool delete = false)
        {
            try
            {
                if (typeToDataMap.TryGetValue(typeof(T), out var data))
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
                    if (typeToDataMap.ContainsKey(typeof(T)))
                    {
                        typeToDataMap.Remove(typeof(T));
                    }
                }
            }

        }

        public bool Get<T>(string key, out T value, bool delete = false)
        {
            try
            {
                if (keyToDataMap.TryGetValue(key, out var data))
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
                    if (keyToDataMap.ContainsKey(key))
                    {
                        keyToDataMap.Remove(key);
                    }
                }
            }
        }
    }
}
