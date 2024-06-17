using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntity<TComponent> : MonoBehaviour, IEntity<TComponent> where TComponent : IComponent
    {
        public virtual string entityId { get; protected set; }
        public virtual Vector3 position { get; set; }
        public virtual Vector3 rotation { get; set; }
        public virtual Vector3 velocity { get; set; }
        public virtual ICollection<TComponent> components { get; protected set; }

        ICollection<IComponent> IEntity.components => components as ICollection<IComponent>;

        IComponent IEntity.AttachComponent(IComponent component)
        {
            return AttachComponent((TComponent)component);
        }

        void IEntity.DetachComponent(IComponent component)
        {
            DetachComponent((TComponent)component);
        }

        public virtual TComponent AttachComponent(TComponent component)
        {
            component.OnAttach(this);

            return component;
        }

        public virtual void DetachComponent(TComponent component)
        {
            component.OnDetach();
        }
    }
}
