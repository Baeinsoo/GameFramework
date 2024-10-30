using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityManager
    {
        IEntity GetEntity(string entityId);
        T GetEntity<T>(string entityId) where T : IEntity;

        IEnumerable<IEntity> GetEntities();
        IEnumerable<T> GetEntities<T>() where T : IEntity;

        void DeleteEntityById(string entityId);

        void UpdateEntities();
    }
}
