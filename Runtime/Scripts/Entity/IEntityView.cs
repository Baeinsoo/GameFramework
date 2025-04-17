using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityView<T> where T : IEntity
    {
        T entity { get; }
    }
}
