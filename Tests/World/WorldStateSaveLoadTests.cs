using System.Numerics;
using GameFramework.World;
using NUnit.Framework;

namespace GameFramework.Tests.World
{
    public class WorldStateSaveLoadTests
    {
        // 게임 훅이 불리는지 세는 최소 월드. WorldBase는 추상이라 테스트용 구체가 필요하다.
        private class TestWorld : WorldBase
        {
            public int SaveGameCalls;
            public int LoadGameCalls;
            public bool LoadGameResult = true;

            public TestWorld(EntityRegistry registry)
                : base(registry, new WorldEventBuffer()) { }

            protected override void SaveGameState(long tick) => SaveGameCalls++;
            protected override bool LoadGameState(long tick)
            {
                LoadGameCalls++;
                return LoadGameResult;
            }
        }

        private static Entity MakeSimulated(string id, float x)
        {
            var e = new Entity(id);
            e.Add(new Simulated());
            e.Add(new GameFramework.World.Transform { Position = new Vector3(x, 0f, 0f) });
            e.Add(new Velocity { Linear = new Vector3(0f, x, 0f) });
            return e;
        }

        [Test]
        public void LoadState_되돌리면_저장한_위치와_속도로_돌아온다()
        {
            var registry = new EntityRegistry();
            var entity = MakeSimulated("a", 1f);
            registry.Add(entity);
            var world = new TestWorld(registry);

            world.SaveState(10);

            entity.Get<GameFramework.World.Transform>().Position = new Vector3(99f, 0f, 0f);
            entity.Get<Velocity>().Linear = new Vector3(0f, 99f, 0f);

            Assert.IsTrue(world.LoadState(10));
            Assert.AreEqual(1f, entity.Get<GameFramework.World.Transform>().Position.X);
            Assert.AreEqual(1f, entity.Get<Velocity>().Linear.Y);
        }

        [Test]
        public void SaveState_Simulated가_없는_엔티티는_담지_않는다()
        {
            var registry = new EntityRegistry();
            var plain = new Entity("b");
            plain.Add(new GameFramework.World.Transform { Position = new Vector3(5f, 0f, 0f) });
            plain.Add(new Velocity());
            registry.Add(plain);
            var world = new TestWorld(registry);

            world.SaveState(10);

            Assert.IsFalse(world.TryGetSavedMotion(10, "b", out _));
        }

        [Test]
        public void LoadState_기록없는_틱이면_false이고_게임훅도_안_부른다()
        {
            var world = new TestWorld(new EntityRegistry());

            Assert.IsFalse(world.LoadState(7));
            Assert.AreEqual(0, world.LoadGameCalls);
        }

        [Test]
        public void LoadState_게임훅이_false면_전체도_false다()
        {
            var registry = new EntityRegistry();
            registry.Add(MakeSimulated("a", 1f));
            var world = new TestWorld(registry) { LoadGameResult = false };

            world.SaveState(10);

            Assert.IsFalse(world.LoadState(10));
        }

        [Test]
        public void SaveState_는_게임훅을_함께_부른다()
        {
            var registry = new EntityRegistry();
            registry.Add(MakeSimulated("a", 1f));
            var world = new TestWorld(registry);

            world.SaveState(10);

            Assert.AreEqual(1, world.SaveGameCalls);
        }

        [Test]
        public void FirstSavedTick_과_LatestSavedTick_이_기록범위를_알려준다()
        {
            var registry = new EntityRegistry();
            registry.Add(MakeSimulated("a", 1f));
            var world = new TestWorld(registry);

            Assert.IsNull(world.FirstSavedTick);

            world.SaveState(10);
            world.SaveState(11);

            Assert.AreEqual(10, world.FirstSavedTick);
            Assert.AreEqual(11, world.LatestSavedTick);
        }
    }
}
