using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class DataStorage : MonoBehaviour
    {
        private Dictionary<string, object> dataMap;

        private void Awake()
        {
            dataMap = new Dictionary<string, object>();
        }

        private void OnDestroy()
        {
            dataMap.Clear();
            dataMap = null;
        }

        public void Set<T>(T value)
        {
            Set("", value);
        }

        public void Set<T>(string key, T value)
        {
            dataMap[key] = value;
        }

        public bool Get<T>(out T value, bool delete = false)
        {
            return Get("", out value, delete);
        }

        public bool Get<T>(string key, out T value, bool delete = false)
        {
            try
            {
                if (dataMap.TryGetValue(key, out var data))
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
                    if (dataMap.ContainsKey(key))
                    {
                        dataMap.Remove(key);
                    }
                }
            }
        }
    }
}
