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

        public IComponent AttachEntityComponent(IComponent component)
        {
            components.Add(component);
            component.OnAttach(this);
            return component;
        }

        public TComponent AttachEntityComponent<TComponent>(TComponent component) where TComponent : IComponent
        {
            components.Add(component);
            component.OnAttach(this);
            return component;
        }

        public void DetachEntityComponent(IComponent component)
        {
            component.OnDetach();

            components.Remove(component);
        }

        public TComponent GetEntityComponent<TComponent>() where TComponent : IComponent
        {
            foreach (var component in components.OrEmpty())
            {
                if (component is TComponent typedComponent)
                {
                    return typedComponent;
                }
            }

            return default;
        }

        public TComponent[] GetEntityComponents<TComponent>() where TComponent : IComponent
        {
            List<TComponent> typedComponents = new List<TComponent>();

            foreach (var component in components.OrEmpty())
            {
                if (component is TComponent typedComponent)
                {
                    typedComponents.Add(typedComponent);
                }
            }

            return typedComponents.ToArray();
        }

        public bool TryGetEntityComponent<TComponent>(out TComponent component) where TComponent : IComponent
        {
            component = GetEntityComponent<TComponent>();
            return component != null;
        }

        public abstract void UpdateEntity();
    }
}
