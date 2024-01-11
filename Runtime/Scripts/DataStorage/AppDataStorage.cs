using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class AppDataStorage : MonoSingleton<AppDataStorage>
    {
        private DataStorage dataStorage;

        protected override void OnAwaked()
        {
            DontDestroyOnLoad(this);

            dataStorage = gameObject.AddComponent<DataStorage>();
        }

        public static void Set<T>(T value)
        {
            instance.dataStorage.Set(value);
        }

        public static void Set<T>(string key, T value)
        {
            instance.dataStorage.Set(key, value);
        }

        public static bool Get<T>(out T value, bool delete = false)
        {
            return instance.dataStorage.Get(out value, delete);
        }

        public static bool Get<T>(string key, out T value, bool delete = false)
        {
            return instance.dataStorage.Get(key, out value, delete);
        }
    }
}

