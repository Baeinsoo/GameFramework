using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFramework
{
    public static partial class Extensions
    {
        public static EntityTransform GetEntityTransform(this IEntity entity)
        {
            return new EntityTransform
            {
                position = entity.position,
                rotation = entity.rotation,
                velocity = entity.velocity
            };
        }

        public static TComponent GetEntityComponent<TComponent>(this IEntity entity) where TComponent : IComponent
        {
            foreach (var component in entity.components.OrEmpty())
            {
                if (component is TComponent typedComponent)
                {
                    return typedComponent;
                }
            }

            return default;
        }

        public static TComponent[] GetEntityComponents<TComponent>(this IEntity entity) where TComponent : IComponent
        {
            List<TComponent> typedComponents = new List<TComponent>();

            foreach (var component in entity.components.OrEmpty())
            {
                if (component is TComponent typedComponent)
                {
                    typedComponents.Add(typedComponent);
                }
            }

            return typedComponents.ToArray();
        }

        public static bool TryGetEntityComponent<TComponent>(this IEntity entity, out TComponent component) where TComponent : IComponent
        {
            component = entity.GetEntityComponent<TComponent>();
            return component != null;
        }

        public static TComponent FindEntityComponent<TComponent>(this IEntity entity, Func<TComponent, bool> predicate) where TComponent : IComponent
        {
            return entity.GetEntityComponents<TComponent>().FirstOrDefault(predicate);
        }

        public static TComponent AddEntityComponent<TComponent>(this MonoEntity entity) where TComponent : MonoComponent
        {
            return entity.AttachEntityComponent(entity.gameObject.AddComponent<TComponent>()) as TComponent;
        }

        public static IComponent AddEntityComponent(this MonoEntity entity, Type type)
        {
            return entity.AttachEntityComponent(entity.gameObject.AddComponent(type) as IComponent);
        }

        // TODO: 고도화 필요!
        [Obsolete("임시 구현: 고도화 필요!")]
        public static bool IsGrounded(this IEntity entity)
        {
            Vector3 checkPosition = entity.position + Vector3.down * 0.2f;
            Collider[] colliders = Physics.OverlapSphere(checkPosition, 0.4f);

            return colliders.Any(col => col.gameObject.name == "Plane");
        }
    }
}
