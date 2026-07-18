using UnityEngine;

namespace GameFramework
{
    public interface IEntityFactory
    {
        TEntity CreateEntity<TEntity, TEntityCreationData>(TEntityCreationData creationData)
            where TEntity : MonoBehaviour
            where TEntityCreationData : struct, IEntityCreationData;
    }
}
