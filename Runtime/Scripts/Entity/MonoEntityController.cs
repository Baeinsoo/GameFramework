using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntityController<T> : MonoBehaviour, IEntityController<T> where T : IEntity
    {
        public T entity { get; protected set; }

        public void SetEntity(T entity)
        {
            this.entity = entity;
        }

        protected virtual void OnDestroy()
        {
            entity = default;
        }
    }
}
