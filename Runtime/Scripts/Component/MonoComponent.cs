using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoComponent<TEntity> : MonoBehaviour, IComponent<TEntity> where TEntity : IEntity
    {
        public virtual TEntity entity { get; protected set; }

        IEntity IComponent.entity => entity as IEntity;

        void IComponent.OnAttach(IEntity entity)
        {
            this.entity = (TEntity)entity;
        }

        public virtual void OnAttach(TEntity entity)
        {
            this.entity = entity;
        }

        public virtual void OnDetach()
        {
            this.entity = default;
        }
    }
}
