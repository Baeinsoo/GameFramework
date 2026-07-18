using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityManager
    {
        TEntity CreateEntity<TEntity, TEntityCreationData>(TEntityCreationData creationData)
            where TEntity : MonoBehaviour
            where TEntityCreationData : struct, IEntityCreationData;

        MonoBehaviour GetEntity(string entityId);
        TEntity GetEntity<TEntity>(string entityId) where TEntity : MonoBehaviour;

        bool TryGetEntity(string entityId, out MonoBehaviour entity);
        bool TryGetEntity<TEntity>(string entityId, out TEntity entity) where TEntity : MonoBehaviour;

        IEnumerable<MonoBehaviour> GetEntities();
        IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : MonoBehaviour;

        void UpdateEntities();

        void DeleteEntityById(string entityId);

        string GetUserIdByEntityId(string entityId);
        TEntity GetEntityByUserId<TEntity>(string userId) where TEntity : MonoBehaviour;

        string GenerateEntityId();
    }
}
