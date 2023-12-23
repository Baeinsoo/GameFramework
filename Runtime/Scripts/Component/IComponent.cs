using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security.Principal;
using UnityEngine;

namespace GameFramework
{
    public interface IComponent<TEntity, TComponent>
        where TEntity : IEntity<TEntity, TComponent>
        where TComponent : IComponent<TEntity, TComponent>
    {
        TEntity entity { get; }

        void OnAttach(TEntity entity);
        void OnDetach();
    }
}
