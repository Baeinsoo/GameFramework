using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework
{
    public class EntityFactory : IEntityFactory
    {
        private readonly Dictionary<(Type entityType, Type creationDataType), IEntityCreator> entityCreators
            = new Dictionary<(Type entityType, Type creationDataType), IEntityCreator>();

        // creator는 DI 컨테이너가 생성·주입해 IEnumerable로 전달한다. (정적 캐시/Activator 없음 →
        // 스코프와 함께 생성·해제되어 룸 재입장 시 stale 참조가 생기지 않는다.)
        public EntityFactory(IEnumerable<IEntityCreator> creators)
        {
            foreach (var creator in creators.OrEmpty())
            {
                var genericInterface = creator.GetType().GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEntityCreator<,>));
                if (genericInterface == null)
                {
                    continue;
                }

                var typeArguments = genericInterface.GetGenericArguments();
                var key = (typeArguments[0], typeArguments[1]);
                entityCreators[key] = creator;
            }
        }

        public TEntity CreateEntity<TEntity, TEntityCreationData>(TEntityCreationData creationData)
            where TEntity : MonoBehaviour
            where TEntityCreationData : struct, IEntityCreationData
        {
            var key = (typeof(TEntity), typeof(TEntityCreationData));

            if (entityCreators.TryGetValue(key, out var creator) == false)
            {
                throw new InvalidOperationException(
                    $"No registered creator found for entity type '{typeof(TEntity).Name}' " +
                    $"and creation data type '{typeof(TEntityCreationData).Name}'. " +
                    "Ensure the appropriate IEntityCreator is registered in the DI container.");
            }

            return (creator as IEntityCreator<TEntity, TEntityCreationData>).Create(creationData);
        }
    }
}
