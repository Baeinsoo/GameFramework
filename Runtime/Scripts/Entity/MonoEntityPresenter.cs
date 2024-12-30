using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntityPresenter<T> : MonoBehaviour, IEntityPresenter<T> where T : IEntity
    {
        public T entity { get; protected set; }
    }
}
