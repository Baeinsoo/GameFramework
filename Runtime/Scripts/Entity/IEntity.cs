using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntity
    {
        string entityId { get; }
        Vector3 position { get; }
        Vector3 rotation { get; }
        Vector3 velocity { get; }
        ICollection<IComponent> components { get; }

        IComponent AttachComponent(IComponent component);
        void DetachComponent(IComponent component);

        void UpdateEntity();
    }
}
