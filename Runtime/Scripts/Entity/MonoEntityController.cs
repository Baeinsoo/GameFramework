using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntityController<T> : MonoBehaviour, IEntityController<T> where T : IEntity
    {
        IEntity IEntityController.entity => entity;

        public T entity { get; protected set; }

        public void SetEntity(T entity)
        {
            this.entity = entity;
        }

        public virtual void Cleanup()
        {
            entity = default;
        }
    }
}
