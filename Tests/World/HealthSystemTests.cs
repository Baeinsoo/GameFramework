using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class HealthSystemTests
    {
        private readonly HealthSystem _system = new HealthSystem();

        [Test]
        public void TakeDamage_reduces_Current()
        {
            var health = new Health(100);

            _system.TakeDamage(health, 30);

            Assert.AreEqual(70, health.Current);
        }

        [Test]
        public void TakeDamage_does_not_go_below_zero()
        {
            var health = new Health(100);

            _system.TakeDamage(health, 150);

            Assert.AreEqual(0, health.Current);
        }

        [Test]
        public void Heal_increases_Current()
        {
            var health = new Health(100) { Current = 50 };

            _system.Heal(health, 20);

            Assert.AreEqual(70, health.Current);
        }

        [Test]
        public void Heal_does_not_exceed_Max()
        {
            var health = new Health(100) { Current = 90 };

            _system.Heal(health, 50);

            Assert.AreEqual(100, health.Current);
        }

        [Test]
        public void SetMax_clamps_Current_when_new_max_is_lower()
        {
            var health = new Health(100);

            _system.SetMax(health, 60);

            Assert.AreEqual(60, health.Current);
        }

        [Test]
        public void SetMax_keeps_Current_when_new_max_is_higher()
        {
            var health = new Health(100) { Current = 80 };

            _system.SetMax(health, 200);

            Assert.AreEqual(80, health.Current);
            Assert.AreEqual(200, health.Max);
        }

        [Test]
        public void ApplyAuthoritativeState_overwrites_Max_and_Current()
        {
            var health = new Health(100) { Current = 40 };

            _system.ApplyAuthoritativeState(health, max: 200, current: 150);

            Assert.AreEqual(200, health.Max);
            Assert.AreEqual(150, health.Current);
        }

        [Test]
        public void ApplyAuthoritativeState_clamps_Current_above_Max()
        {
            var health = new Health(100);

            _system.ApplyAuthoritativeState(health, max: 80, current: 999);

            Assert.AreEqual(80, health.Max);
            Assert.AreEqual(80, health.Current);
        }

        [Test]
        public void ApplyAuthoritativeState_clamps_negative_Current_to_zero()
        {
            var health = new Health(100);

            _system.ApplyAuthoritativeState(health, max: 100, current: -5);

            Assert.AreEqual(0, health.Current);
        }
    }
}
