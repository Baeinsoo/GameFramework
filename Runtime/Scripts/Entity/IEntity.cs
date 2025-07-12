using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntity
    {
        string entityId { get; }
        Vector3 position { get; set; }
        Vector3 rotation { get; set; }
        Vector3 velocity { get; set; }
        ICollection<IComponent> components { get; }

        IComponent AttachEntityComponent(IComponent component);
        void DetachEntityComponent(IComponent component);

        void UpdateEntity();
    }
}
