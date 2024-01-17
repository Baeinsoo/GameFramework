using GameFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntity<TEntity, TComponent> : MonoBehaviour, IEntity<TEntity, TComponent>
        where TEntity : MonoEntity<TEntity, TComponent>
        where TComponent : MonoComponent<TEntity, TComponent>
    {
        public virtual string entityId { get; protected set; }
        public virtual Vector3 position { get; set; }
        public virtual Vector3 rotation { get; set; }
        public virtual Vector3 velocity { get; set; }
        public virtual ICollection<TComponent> components { get; protected set; }

        public virtual T AttachComponent<T>(T component) where T : TComponent
        {
            component.OnAttach(this as TEntity);

            return component;
        }

        public virtual void DetachComponent(TComponent component)
        {
            component.OnDetach();
        }
    }
}
