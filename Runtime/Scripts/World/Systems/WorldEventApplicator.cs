using System.Collections.Generic;

namespace GameFramework.World
{
    /// <summary>
    /// WorldEventBuffer의 스냅샷을 받아 이벤트 타입별로 dispatch해 상태에 쓴다.
    /// EntityRegistry에 없는 targetId는 조용히 건너뛴다 (네트워크 race나 비등록 엔티티 대응).
    /// 슬라이스 3 처리 이벤트: DamageDealtEvent (→ HealthSystem.ApplyDamageDealt), DeathEvent (no-op).
    /// </summary>
    public class WorldEventApplicator
    {
        private readonly EntityRegistry _registry;
        private readonly HealthSystem _healthSystem;

        public WorldEventApplicator(EntityRegistry registry, HealthSystem healthSystem)
        {
            _registry = registry;
            _healthSystem = healthSystem;
        }

        public void Apply(IReadOnlyList<WorldEvent> events)
        {
            foreach (var e in events)
            {
                switch (e)
                {
                    case DamageDealtEvent dde:
                        ApplyDamageDealt(dde);
                        break;
                    case DeathEvent:
                        // 슬라이스 3: state 변화 없음. Stage ④에서 Death 컴포넌트가 생기면 여기서 처리.
                        break;
                }
            }
        }

        private void ApplyDamageDealt(DamageDealtEvent e)
        {
            var entity = _registry.Get(e.targetId);
            if (entity == null) return;

            var health = entity.Get<Health>();
            if (health == null) return;

            _healthSystem.ApplyDamageDealt(health, e);
        }
    }
}
