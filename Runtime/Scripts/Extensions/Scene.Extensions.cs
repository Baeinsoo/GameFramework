using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameFramework
{
    public static partial class Extensions
    {
        public static IEnumerable<GameObject> FindGameObjectsWithAttribute<T>(this Scene self) where T : Attribute
        {
            return self.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<MonoBehaviour>(true))
                .Where(mb => mb.GetType().GetCustomAttribute<T>() != null)
                .Select(mb => mb.gameObject)
                .Distinct();
        }

        public static IEnumerable<MonoBehaviour> FindComponentsWithAttribute<T>(this Scene self) where T : Attribute
        {
            return self.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<MonoBehaviour>(true))
                .Where(mb => mb.GetType().GetCustomAttribute<T>() != null)
                .Distinct();
        }
    }
}
