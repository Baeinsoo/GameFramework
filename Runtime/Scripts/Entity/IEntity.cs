using UnityEngine;

namespace GameFramework
{
    public interface IEntity
    {
        string entityId { get; }
        Vector3 position { get; set; }
        Vector3 rotation { get; set; }
        Vector3 velocity { get; set; }

        void UpdateEntity();
    }
}
