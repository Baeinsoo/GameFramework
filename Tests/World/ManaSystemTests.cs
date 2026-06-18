using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class ManaSystemTests
    {
        private readonly ManaSystem _system = new ManaSystem();

        [Test]
        public void ApplyAuthoritativeState_overwrites_Max_and_Current()
        {
            var mana = new Mana(100) { Current = 40 };

            _system.ApplyAuthoritativeState(mana, max: 200, current: 150);

            Assert.AreEqual(200, mana.Max);
            Assert.AreEqual(150, mana.Current);
        }

        [Test]
        public void ApplyAuthoritativeState_clamps_Current_above_Max()
        {
            var mana = new Mana(100);

            _system.ApplyAuthoritativeState(mana, max: 80, current: 999);

            Assert.AreEqual(80, mana.Max);
            Assert.AreEqual(80, mana.Current);
        }

        [Test]
        public void ApplyAuthoritativeState_clamps_negative_Current_to_zero()
        {
            var mana = new Mana(100);

            _system.ApplyAuthoritativeState(mana, max: 100, current: -5);

            Assert.AreEqual(0, mana.Current);
        }
    }
}
