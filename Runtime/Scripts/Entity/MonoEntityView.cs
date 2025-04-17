using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntityView<T> : MonoBehaviour, IEntityView<T> where T : IEntity
    {
        public T entity { get; protected set; }
    }
}
