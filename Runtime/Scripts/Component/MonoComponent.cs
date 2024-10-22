using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public class MonoComponent : MonoBehaviour, IComponent
    {
        public IEntity entity { get; protected set; }

        public virtual void OnAttach(IEntity entity)
        {
            this.entity = entity;
        }

        public virtual void OnDetach()
        {
            this.entity = null;
        }
    }
}
