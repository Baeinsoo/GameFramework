using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class EntityTests
    {
        private class TestComponent : Component { }

        [Test]
        public void Get_returns_component_after_Add()
        {
            var entity = new Entity("e1");
            var component = new TestComponent();

            entity.Add(component);

            Assert.AreSame(component, entity.Get<TestComponent>());
        }

        [Test]
        public void Add_sets_Owner_to_the_entity()
        {
            var entity = new Entity("e1");
            var component = new TestComponent();

            entity.Add(component);

            Assert.AreSame(entity, component.Owner);
        }

        [Test]
        public void Has_returns_true_after_Add()
        {
            var entity = new Entity("e1");
            entity.Add(new TestComponent());

            Assert.IsTrue(entity.Has<TestComponent>());
        }

        [Test]
        public void Has_returns_false_when_component_not_added()
        {
            var entity = new Entity("e1");

            Assert.IsFalse(entity.Has<TestComponent>());
        }

        [Test]
        public void Get_returns_null_when_component_not_added()
        {
            var entity = new Entity("e1");

            Assert.IsNull(entity.Get<TestComponent>());
        }

        [Test]
        public void Remove_makes_Has_return_false()
        {
            var entity = new Entity("e1");
            entity.Add(new TestComponent());

            entity.Remove<TestComponent>();

            Assert.IsFalse(entity.Has<TestComponent>());
        }

        [Test]
        public void Remove_clears_Owner()
        {
            var entity = new Entity("e1");
            var component = new TestComponent();
            entity.Add(component);

            entity.Remove<TestComponent>();

            Assert.IsNull(component.Owner);
        }

        [Test]
        public void Remove_returns_true_when_component_present()
        {
            var entity = new Entity("e1");
            entity.Add(new TestComponent());

            Assert.IsTrue(entity.Remove<TestComponent>());
        }

        [Test]
        public void Remove_returns_false_when_component_absent()
        {
            var entity = new Entity("e1");

            Assert.IsFalse(entity.Remove<TestComponent>());
        }

        [Test]
        public void Add_same_type_replaces_previous_component()
        {
            var entity = new Entity("e1");
            var first = new TestComponent();
            var second = new TestComponent();

            entity.Add(first);
            entity.Add(second);

            Assert.AreSame(second, entity.Get<TestComponent>());
        }
    }
}
