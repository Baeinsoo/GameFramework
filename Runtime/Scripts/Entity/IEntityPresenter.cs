using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IEntityPresenter<T> where T : IEntity
    {
        T entity { get; }
    }
}
