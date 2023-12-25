using GameFramework;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using UnityEngine;

namespace GameFramework
{
    public interface IEntity<TEntity, TComponent>
        where TEntity : IEntity<TEntity, TComponent>
        where TComponent : IComponent<TEntity, TComponent>
    {
        string entityId { get; }
        Vector3 position { get; }
        Vector3 rotation { get; }
        IEnumerable<TComponent> components { get; }

        void AttachComponent(TComponent component);
        void DetachComponent(TComponent component);
    }
}
