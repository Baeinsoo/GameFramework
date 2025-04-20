using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntityView<TEntity, TEntityController> : MonoBehaviour, IEntityView<TEntity, TEntityController> 
        where TEntity : IEntity
        where TEntityController : IEntityController<TEntity>
    {
        public TEntity entity { get; protected set; }
        public TEntityController entityController { get; protected set; }

        public void SetEntity(TEntity entity)
        {
            this.entity = entity;
        }

        public void SetEntityController(TEntityController entityController)
        {
            this.entityController = entityController;
        }

        protected virtual void OnDestroy()
        {
            entity = default;
            entityController = default;
        }
    }
}
