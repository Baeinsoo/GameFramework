using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityCreationData
    {
        string entityId { get; set; }
        Vector3 position { get; set; }
        Vector3 rotation { get; set; }
        Vector3 velocity { get; set; }
    }
}
