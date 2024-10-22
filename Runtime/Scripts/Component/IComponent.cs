using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameFramework
{
    public interface IComponent
    {
        IEntity entity { get; }

        void OnAttach(IEntity entity);
        void OnDetach();
    }
}
