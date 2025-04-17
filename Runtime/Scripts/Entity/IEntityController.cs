using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityController<T> where T : IEntity
    {
        T entity { get; }
    }
}
