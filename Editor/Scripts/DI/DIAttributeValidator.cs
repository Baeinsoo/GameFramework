using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace GameFramework.Editor
{
    public static class DIAttributeValidator
    {
        [InitializeOnLoadMethod]
        static void ValidateAttributes()
        {
            Type[] attributeTypes = new Type[]
            {
                typeof(DIGameObjectAttribute),
                typeof(DIMonoBehaviourAttribute),
            };

            foreach (var attributeType in attributeTypes)
            {
                ValidateDIAttribute(attributeType);
            }
        }

        private static void ValidateDIAttribute(Type attributeType)
        {
            var invalidTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.GetCustomAttribute(attributeType) != null)
                .Where(t => typeof(MonoBehaviour).IsAssignableFrom(t) == false);

            foreach (var type in invalidTypes.OrEmpty())
            {
                Debug.LogError($"[{type.Name}] with {attributeType.Name} must inherit from MonoBehaviour");
            }
        }
    }
}
