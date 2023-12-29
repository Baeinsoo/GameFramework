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
        ICollection<TComponent> components { get; }

        T AttachComponent<T>(T component) where T : TComponent;
        void DetachComponent(TComponent component);
    }
}
