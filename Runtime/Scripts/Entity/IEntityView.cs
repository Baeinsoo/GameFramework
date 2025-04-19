using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityView<TEntity, TEntityController>
        where TEntity : IEntity
        where TEntityController : IEntityController<TEntity>
    {
        TEntity entity { get; }
        TEntityController entityController { get; }
    }
}
