using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace GameFramework
{
    public class MonoComponent<TEntity, TComponent> : MonoBehaviour, IComponent<TEntity, TComponent>
        where TEntity : MonoEntity<TEntity, TComponent>
        where TComponent : MonoComponent<TEntity, TComponent>
    {
        public virtual TEntity entity { get; protected set; }

        public virtual void OnAttach(TEntity entity)
        {
            this.entity = entity;
        }

        public virtual void OnDetach()
        {
            this.entity = null;
        }
    }
}
