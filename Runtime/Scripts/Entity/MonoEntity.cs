using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public abstract class MonoEntity : MonoBehaviour, IEntity
    {
        public string entityId { get; protected set; }
        public virtual Vector3 position { get; set; }
        public virtual Vector3 rotation { get; set; }
        public virtual Vector3 velocity { get; set; }
        public virtual ICollection<IComponent> components { get; } = new List<IComponent>();

        public virtual IComponent AttachComponent(IComponent component)
        {
            components.Add(component);

            component.OnAttach(this);
            return component;
        }

        public virtual void DetachComponent(IComponent component)
        {
            component.OnDetach();

            components.Remove(component);
        }

        public abstract void UpdateEntity();
    }
}
