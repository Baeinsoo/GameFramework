using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoEntityPresenter : MonoBehaviour, IEntityPresenter
    {
        public IEntity entity { get; protected set; }
    }
}
