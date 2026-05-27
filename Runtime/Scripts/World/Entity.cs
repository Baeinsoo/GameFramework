using System;
using System.Collections.Generic;

namespace GameFramework.World
{
    /// <summary>
    /// CBD 엔티티. 빈 컨테이너이며 능력은 <see cref="Component"/> 구현체를 조합하여 구성한다.
    /// 컴포넌트는 타입당 하나만 보유한다.
    /// </summary>
    public class Entity
    {
        private readonly Dictionary<Type, Component> _components = new Dictionary<Type, Component>();

        public string Id { get; }

        public Entity(string id)
        {
            Id = id;
        }

        /// <summary>컴포넌트를 추가하고 Owner를 이 엔티티로 설정한다. 같은 타입이 이미 있으면 교체한다.</summary>
        public void Add<T>(T component) where T : Component
        {
            _components[typeof(T)] = component;
            component.Owner = this;
        }

        /// <summary>타입으로 컴포넌트를 조회한다. 없으면 null.</summary>
        public T Get<T>() where T : Component
        {
            _components.TryGetValue(typeof(T), out var component);
            return component as T;
        }

        /// <summary>해당 타입 컴포넌트의 존재 여부.</summary>
        public bool Has<T>() where T : Component
        {
            return _components.ContainsKey(typeof(T));
        }

        /// <summary>컴포넌트를 제거하고 Owner를 해제한다. 제거되면 true, 없었으면 false.</summary>
        public bool Remove<T>() where T : Component
        {
            if (_components.TryGetValue(typeof(T), out var component))
            {
                component.Owner = null;
                return _components.Remove(typeof(T));
            }

            return false;
        }
    }
}
