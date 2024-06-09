using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework
{
    [CreateAssetMenu(fileName = "PrefabReferences", menuName = "GameFramework/PrefabReferences")]
    public class PrefabReferences : ScriptableObjectSingleton<PrefabReferences>
    {
        [SerializeField] private GameObject[] prefabs;

        private Dictionary<string, GameObject> prefabMap;

        private void Awake()
        {
            prefabMap = new Dictionary<string, GameObject>();

            foreach (var prefab in prefabs ?? Enumerable.Empty<GameObject>())
            {
                prefabMap[prefab.name] = prefab;
            }
        }

        public T Instantiate<T>(Transform parent = null)
        {
            return Instantiate<T>(null, parent);
        }

        public T Instantiate<T>(string name, Transform parent = null)
        {
            var instance = Instantiate(name ?? typeof(T).Name, parent);
            if (instance == null)
            {
                return default;
            }
            return instance.GetComponentInChildren<T>();
        }

        public GameObject Instantiate(string name, Transform parent = null)
        {
            if (prefabMap.TryGetValue(name, out var source))
            {
                return GameObject.Instantiate(source, parent, false);
            }
            else
            {
                Debug.LogWarning($"There is no corresponding Prefab. name: {name}");
                return default;
            }
        }
    }
}
