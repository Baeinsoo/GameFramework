using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class HealthTests
    {
        [Test]
        public void Constructor_sets_Current_to_Max()
        {
            var health = new Health(100);

            Assert.AreEqual(100, health.Current);
        }

        [Test]
        public void IsAlive_is_true_when_Current_above_zero()
        {
            var health = new Health(100);

            Assert.IsTrue(health.IsAlive);
        }

        [Test]
        public void IsAlive_is_false_when_Current_is_zero()
        {
            var health = new Health(100) { Current = 0 };

            Assert.IsFalse(health.IsAlive);
        }

        [Test]
        public void IsDead_is_true_when_Current_is_zero()
        {
            var health = new Health(100) { Current = 0 };

            Assert.IsTrue(health.IsDead);
        }
    }
}
