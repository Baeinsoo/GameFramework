using System;
using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class EntityRegistryTests
    {
        [Test]
        public void Add_then_Get_returns_same_entity()
        {
            var registry = new EntityRegistry();
            var entity = new Entity("e1");

            registry.Add(entity);

            Assert.AreSame(entity, registry.Get("e1"));
        }

        [Test]
        public void Add_increments_Count()
        {
            var registry = new EntityRegistry();

            registry.Add(new Entity("e1"));
            registry.Add(new Entity("e2"));

            Assert.AreEqual(2, registry.Count);
        }

        [Test]
        public void TryGet_returns_true_with_entity_when_present()
        {
            var registry = new EntityRegistry();
            var entity = new Entity("e1");
            registry.Add(entity);

            var found = registry.TryGet("e1", out var result);

            Assert.IsTrue(found);
            Assert.AreSame(entity, result);
        }

        [Test]
        public void TryGet_returns_false_with_null_when_absent()
        {
            var registry = new EntityRegistry();

            var found = registry.TryGet("missing", out var result);

            Assert.IsFalse(found);
            Assert.IsNull(result);
        }

        [Test]
        public void Get_returns_null_when_absent()
        {
            var registry = new EntityRegistry();

            Assert.IsNull(registry.Get("missing"));
        }

        [Test]
        public void Contains_reflects_membership()
        {
            var registry = new EntityRegistry();
            registry.Add(new Entity("e1"));

            Assert.IsTrue(registry.Contains("e1"));
            Assert.IsFalse(registry.Contains("e2"));
        }

        [Test]
        public void Remove_returns_true_and_removes_when_present()
        {
            var registry = new EntityRegistry();
            registry.Add(new Entity("e1"));

            Assert.IsTrue(registry.Remove("e1"));
            Assert.IsFalse(registry.Contains("e1"));
            Assert.AreEqual(0, registry.Count);
        }

        [Test]
        public void Remove_returns_false_when_absent()
        {
            var registry = new EntityRegistry();

            Assert.IsFalse(registry.Remove("missing"));
        }

        [Test]
        public void All_returns_every_added_entity()
        {
            var registry = new EntityRegistry();
            var a = new Entity("a");
            var b = new Entity("b");
            registry.Add(a);
            registry.Add(b);

            CollectionAssert.AreEquivalent(new[] { a, b }, registry.All);
        }

        [Test]
        public void Add_throws_on_null_entity()
        {
            var registry = new EntityRegistry();

            Assert.Throws<ArgumentNullException>(() => registry.Add(null));
        }

        [Test]
        public void Add_throws_when_entity_Id_is_null()
        {
            var registry = new EntityRegistry();

            Assert.Throws<ArgumentException>(() => registry.Add(new Entity(null)));
        }

        [Test]
        public void Add_throws_on_duplicate_Id()
        {
            var registry = new EntityRegistry();
            registry.Add(new Entity("e1"));

            Assert.Throws<ArgumentException>(() => registry.Add(new Entity("e1")));
        }
    }
}
