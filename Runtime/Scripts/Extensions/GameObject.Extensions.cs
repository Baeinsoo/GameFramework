using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GameFramework
{
    public static partial class Extensions
    {
        public static T GetOrAddComponent<T>(this GameObject self) where T : Component
        {
            return self.GetComponent<T>() ?? self.AddComponent<T>();
        }

        public static GameObject CreateChildWithComponents(this GameObject parent, string name = null, params Type[] componentTypes)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);

            foreach (var componentType in componentTypes.OrEmpty())
            {
                go.AddComponent(componentType);
            }

            return go;
        }

        public static T CreateChildWithComponent<T>(this GameObject parent, string name = null) where T : Component
        {
            var go = CreateChildWithComponents(parent, name ?? typeof(T).Name, typeof(T));

            return go.GetComponent<T>();
        }

        public static GameObject CreateChild(this GameObject parent, string name = null)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform, false);
            return go;
        }
    }
}
