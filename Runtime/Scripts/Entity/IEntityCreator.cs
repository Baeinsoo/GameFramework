using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityCreator { }

    public interface IEntityCreator<out TEntity, in TEntityCreationData> : IEntityCreator
        where TEntity : IEntity
        where TEntityCreationData : IEntityCreationData
    {
        TEntity Create(TEntityCreationData data);
    }
}
