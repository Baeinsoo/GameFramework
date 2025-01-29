using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework
{
    public static class EntityFactory
    {
        private static readonly Dictionary<(Type entityType, Type creationDataType), IEntityCreator> entityCreators;

        static EntityFactory()
        {
            entityCreators = new Dictionary<(Type entityType, Type creationDataType), IEntityCreator>();

            RegisterEntityCreators();
        }

        private static void RegisterEntityCreators()
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
            foreach (var type in types.OrEmpty())
            {
                var attribute = (EntityCreatorRegistrationAttribute)Attribute.GetCustomAttribute(type, typeof(EntityCreatorRegistrationAttribute));
                if (attribute == null || attribute.value == false)
                {
                    continue;
                }

                var genericInterface = type.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityCreator<,>));
                if (genericInterface == null || type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                if (Activator.CreateInstance(type) is IEntityCreator instance)
                {
                    var typeArguments = genericInterface.GetGenericArguments();
                    var entityType = typeArguments[0];
                    var creationDataType = typeArguments[1];
                    var key = (entityType, creationDataType);

                    entityCreators[key] = instance;
                    Debug.Log($"Registered Creator: {type.Name} for {entityType.Name} and {creationDataType.Name}");
                }
            }
        }

        public static TEntity CreateEntity<TEntity, TEntityCreationData>(TEntityCreationData creationData)
            where TEntity : IEntity
            where TEntityCreationData : struct, IEntityCreationData
        {
            var key = (typeof(TEntity), typeof(TEntityCreationData));

            if (entityCreators.TryGetValue(key, out var creator) == false)
            {
                throw new InvalidOperationException(
                    $"No registered creator found for entity type '{typeof(TEntity).Name}' " +
                    $"and creation data type '{typeof(TEntityCreationData).Name}'. " +
                    "Ensure the appropriate IEntityCreator is registered."
                );
            }

            return (creator as IEntityCreator<TEntity, TEntityCreationData>).Create(creationData);
        }
    }
}
