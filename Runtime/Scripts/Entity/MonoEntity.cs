using UnityEngine;

namespace GameFramework
{
    public abstract class MonoEntity : MonoBehaviour, IEntity
    {
        public string entityId { get; protected set; }
        public virtual Vector3 position { get; set; }
        public virtual Vector3 rotation { get; set; }
        public virtual Vector3 velocity { get; set; }

        public abstract void UpdateEntity();
    }
}
