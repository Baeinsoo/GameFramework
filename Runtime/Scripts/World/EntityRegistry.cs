using System;
using System.Collections.Generic;

namespace GameFramework.World
{
    /// <summary>
    /// Id를 키로 <see cref="Entity"/>를 관리하는 컬렉션. 순수 C# 컨테이너이며 엔진 의존성이 없다.
    /// 등록된 엔티티들의 단일 원천(source-of-truth)이며, 중복 Id 등록은 버그를 나타내므로 예외를 던진다.
    /// </summary>
    public class EntityRegistry
    {
        private readonly Dictionary<string, Entity> _entities = new Dictionary<string, Entity>();

        /// <summary>등록된 엔티티의 총 수.</summary>
        public int Count => _entities.Count;

        /// <summary>등록된 엔티티의 라이브 뷰. 스냅샷이 아니므로 레지스트리가 변경되는 동안 열거하면 동작이 정의되지 않는다. 순서는 보장하지 않는다.</summary>
        public IEnumerable<Entity> All => _entities.Values;

        /// <summary>엔티티를 Id로 등록한다. entity가 null이면 <see cref="ArgumentNullException"/>, entity.Id가 null이면 <see cref="ArgumentException"/>, 동일 Id가 이미 등록되어 있으면 <see cref="ArgumentException"/>을 던진다.</summary>
        public void Add(Entity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (entity.Id == null)
            {
                throw new ArgumentException("Entity.Id must not be null", nameof(entity));
            }
            if (_entities.ContainsKey(entity.Id))
            {
                throw new ArgumentException($"Entity with Id '{entity.Id}' already registered", nameof(entity));
            }

            _entities.Add(entity.Id, entity);
        }

        /// <summary>id에 해당하는 엔티티를 제거한다. 제거되면 true, 없었으면 false.</summary>
        public bool Remove(string id) => _entities.Remove(id);

        /// <summary>id에 해당하는 엔티티를 반환한다. 없으면 null.</summary>
        public Entity Get(string id)
        {
            _entities.TryGetValue(id, out var entity);
            return entity;
        }

        /// <summary>id에 해당하는 엔티티가 있으면 true와 엔티티를 반환하고, 없으면 false와 null을 반환한다.</summary>
        public bool TryGet(string id, out Entity entity) => _entities.TryGetValue(id, out entity);

        /// <summary>id에 해당하는 엔티티가 등록되어 있으면 true.</summary>
        public bool Contains(string id) => _entities.ContainsKey(id);
    }
}
