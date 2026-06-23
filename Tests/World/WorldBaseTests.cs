using System.Collections.Generic;
using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class WorldBaseTests
    {
        private class RecordingWorld : WorldBase
        {
            public readonly List<string> Calls = new List<string>();

            public RecordingWorld(EntityRegistry registry, WorldEventBuffer buffer) : base(registry, buffer) { }

            protected override void Collection(long tick, float dt) => Calls.Add("Collection");
            protected override void Mutation(long tick, float dt) => Calls.Add("Mutation");
            protected override void Detection(long tick, float dt) => Calls.Add("Detection");
        }

        [Test]
        public void Tick_calls_phases_in_order_once_each()
        {
            var world = new RecordingWorld(new EntityRegistry(), new WorldEventBuffer());

            world.Tick(0, 0.016f);

            Assert.AreEqual(new[] { "Collection", "Mutation", "Detection" }, world.Calls);
        }

        [Test]
        public void Exposes_injected_state()
        {
            var registry = new EntityRegistry();
            var buffer = new WorldEventBuffer();

            var world = new RecordingWorld(registry, buffer);

            Assert.AreSame(registry, world.EntityRegistry);
            Assert.AreSame(buffer, world.EventBuffer);
        }
    }
}
