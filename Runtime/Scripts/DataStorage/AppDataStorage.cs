using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class AppDataStorage : MonoSingleton<AppDataStorage>, IDataStorage
    {
        private IDataStorage dataStorageImpl;

        protected override void OnAwaked()
        {
            DontDestroyOnLoad(this);

            dataStorageImpl = gameObject.AddComponent<DataStorage>();
        }

        public void Set<T>(T value)
        {
            dataStorageImpl.Set(value);
        }

        public void Set<T>(string key, T value)
        {
            dataStorageImpl.Set(key, value);
        }

        public bool Get<T>(out T value, bool delete = false)
        {
            return dataStorageImpl.Get(out value, delete);
        }

        public bool Get<T>(string key, out T value, bool delete = false)
        {
            return dataStorageImpl.Get(key, out value, delete);
        }
    }
}
