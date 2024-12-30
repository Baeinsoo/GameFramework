using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityManager
    {
        TEntity CreateEntity<TEntity, TEntityCreationData>(TEntityCreationData creationData) where TEntity : IEntity where TEntityCreationData : IEntityCreationData;

        IEntity GetEntity(string entityId);
        TEntity GetEntity<TEntity>(string entityId) where TEntity : IEntity;

        IEnumerable<IEntity> GetEntities();
        IEnumerable<TEntity> GetEntities<TEntity>() where TEntity : IEntity;

        void UpdateEntities();

        void DeleteEntityById(string entityId);
    }
}
