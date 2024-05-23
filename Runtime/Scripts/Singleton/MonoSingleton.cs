using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace GameFramework
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static bool applicationQuitting;

        private static T _instance;
        public static T instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType(typeof(T)) as T;

                    if (_instance == null)
                    {
                        Instantiate();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnApplicationQuit()
        {
            applicationQuitting = true;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
            }
        }

        public static void Instantiate()
        {
            if (_instance != null)
            {
                Debug.LogWarning($"There is an instance already. Instantiate is ignored.");
                return;
            }

            if (applicationQuitting == true)
            {
                throw new UnityException("Can not instantiate. Application is quitting.");
            }

            var factoryMethod = typeof(T).GetMethod("CreateInstance", BindingFlags.Public | BindingFlags.Static);
            if (factoryMethod != null)
            {
                _instance = factoryMethod.Invoke(null, null) as T;
            }
            else
            {
                var go = new GameObject($"{typeof(T).Name} Singleton");

                _instance = go.AddComponent<T>();
            }

            var attribute = _instance.GetType().GetCustomAttribute(typeof(DontDestroyMonoSingletonAttribute));
            if (attribute != null)
            {
                DontDestroyOnLoad(_instance);
            }
        }

        public static bool HasInstance()
        {
            return _instance != null;
        }
    }
}
