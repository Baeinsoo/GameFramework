using NUnit.Framework;
using System.Collections.Generic;

namespace GameFramework.World.Tests
{
    public class WorldEventApplicatorTests
    {
        private EntityRegistry _registry;
        private HealthSystem _healthSystem;
        private WorldEventApplicator _applicator;

        [SetUp]
        public void SetUp()
        {
            _registry = new EntityRegistry();
            _healthSystem = new HealthSystem();
            _applicator = new WorldEventApplicator(_registry, _healthSystem);
        }

        [Test]
        public void Apply_DamageDealtEvent_writes_remaining_to_Health()
        {
            var entity = new Entity("1");
            entity.Add(new Health(100));
            _registry.Add(entity);

            var events = new List<WorldEvent> {
                new DamageDealtEvent("1", "2", 30, false, false, 70, false)
            };

            _applicator.Apply(events);

            Assert.AreEqual(70, entity.Get<Health>().Current);
        }

        [Test]
        public void Apply_DamageDealtEvent_for_unregistered_target_does_not_throw()
        {
            var events = new List<WorldEvent> {
                new DamageDealtEvent("missing-id", "2", 30, false, false, 70, false)
            };

            Assert.DoesNotThrow(() => _applicator.Apply(events));
        }

        [Test]
        public void Apply_DamageDealtEvent_for_entity_without_Health_does_not_throw()
        {
            var entity = new Entity("1");
            _registry.Add(entity);

            var events = new List<WorldEvent> {
                new DamageDealtEvent("1", "2", 30, false, false, 70, false)
            };

            Assert.DoesNotThrow(() => _applicator.Apply(events));
        }

        [Test]
        public void Apply_DeathEvent_is_noop_for_state()
        {
            var entity = new Entity("1");
            entity.Add(new Health(100) { Current = 0 });
            _registry.Add(entity);

            var events = new List<WorldEvent> { new DeathEvent("1", "2") };

            _applicator.Apply(events);

            Assert.AreEqual(0, entity.Get<Health>().Current);
            Assert.AreEqual(100, entity.Get<Health>().Max);
        }

        [Test]
        public void Apply_processes_events_in_snapshot_order()
        {
            var entity = new Entity("1");
            entity.Add(new Health(100));
            _registry.Add(entity);

            var events = new List<WorldEvent> {
                new DamageDealtEvent("1", "2", 30, false, false, 70, false),
                new DamageDealtEvent("1", "2", 20, false, false, 50, false),
                new DamageDealtEvent("1", "2", 50, false, false, 0,  true),
            };

            _applicator.Apply(events);

            Assert.AreEqual(0, entity.Get<Health>().Current);
        }
    }
}
