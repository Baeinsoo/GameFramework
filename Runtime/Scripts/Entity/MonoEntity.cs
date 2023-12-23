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
        public virtual Vector3 position { get; protected set; }
        public virtual Vector3 rotation { get; protected set; }
        public virtual IEnumerable<TComponent> components { get; protected set; }
    }
}
