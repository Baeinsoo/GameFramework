using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class StatsSystemTests
    {
        private enum Stat { Attack = 1, Defense = 2 }

        private readonly StatsSystem _system = new StatsSystem();

        private static StatModifier Flat(Stat stat, float value, string source = "src")
            => new StatModifier((int)stat, value, ModifierType.Flat, source);

        [Test]
        public void GetValue_returns_base_when_no_modifiers()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;

            Assert.AreEqual(100f, _system.GetValue(stats, (int)Stat.Attack));
        }

        [Test]
        public void GetValue_returns_zero_when_stat_unset()
        {
            var stats = new Stats();

            Assert.AreEqual(0f, _system.GetValue(stats, (int)Stat.Attack));
        }

        [Test]
        public void Flat_modifier_adds_to_base()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;
            _system.AddModifier(stats, Flat(Stat.Attack, 20f));

            Assert.AreEqual(120f, _system.GetValue(stats, (int)Stat.Attack));
        }

        [Test]
        public void Multiple_flat_modifiers_sum()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;
            _system.AddModifier(stats, Flat(Stat.Attack, 20f));
            _system.AddModifier(stats, Flat(Stat.Attack, 30f));

            Assert.AreEqual(150f, _system.GetValue(stats, (int)Stat.Attack));
        }

        [Test]
        public void PercentAdd_modifiers_sum_then_scale()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;
            _system.AddModifier(stats, new StatModifier((int)Stat.Attack, 0.2f, ModifierType.PercentAdd, "a"));
            _system.AddModifier(stats, new StatModifier((int)Stat.Attack, 0.3f, ModifierType.PercentAdd, "b"));

            Assert.AreEqual(150f, _system.GetValue(stats, (int)Stat.Attack), 0.0001f); // 100 * (1 + 0.5)
        }

        [Test]
        public void PercentMult_modifiers_apply_multiplicatively()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;
            _system.AddModifier(stats, new StatModifier((int)Stat.Attack, 0.2f, ModifierType.PercentMult, "a"));

            Assert.AreEqual(120f, _system.GetValue(stats, (int)Stat.Attack), 0.0001f); // 100 * 1.2
        }

        [Test]
        public void Modifiers_apply_in_order_flat_then_percentAdd_then_percentMult()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;
            _system.AddModifier(stats, new StatModifier((int)Stat.Attack, 50f, ModifierType.Flat, "f"));
            _system.AddModifier(stats, new StatModifier((int)Stat.Attack, 0.10f, ModifierType.PercentAdd, "pa"));
            _system.AddModifier(stats, new StatModifier((int)Stat.Attack, 0.20f, ModifierType.PercentMult, "pm"));

            // (100 + 50) * (1 + 0.10) * (1 + 0.20) = 150 * 1.1 * 1.2 = 198
            Assert.AreEqual(198f, _system.GetValue(stats, (int)Stat.Attack), 0.0001f);
        }

        [Test]
        public void Modifiers_for_other_stats_are_ignored()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;
            _system.AddModifier(stats, Flat(Stat.Defense, 50f));

            Assert.AreEqual(100f, _system.GetValue(stats, (int)Stat.Attack));
        }

        [Test]
        public void RemoveModifiersBySourceId_removes_only_that_source()
        {
            var stats = new Stats();
            stats.BaseStats[(int)Stat.Attack] = 100f;
            _system.AddModifier(stats, Flat(Stat.Attack, 20f, "buffA"));
            _system.AddModifier(stats, Flat(Stat.Attack, 30f, "buffB"));

            _system.RemoveModifiersBySourceId(stats, "buffA");

            Assert.AreEqual(130f, _system.GetValue(stats, (int)Stat.Attack)); // 100 + 30
        }

        [Test]
        public void RemoveModifiersBySourceId_returns_true_when_removed_and_false_otherwise()
        {
            var stats = new Stats();
            _system.AddModifier(stats, Flat(Stat.Attack, 20f, "buffA"));

            Assert.IsTrue(_system.RemoveModifiersBySourceId(stats, "buffA"));
            Assert.IsFalse(_system.RemoveModifiersBySourceId(stats, "nope"));
        }

        [Test]
        public void SetBase_sets_base_value_reflected_in_GetValue()
        {
            var stats = new Stats();

            _system.SetBase(stats, (int)EntityStatType.Strength, 7f);

            Assert.AreEqual(7f, _system.GetValue(stats, (int)EntityStatType.Strength));
        }

        [Test]
        public void SetBase_overwrites_existing_base()
        {
            var stats = new Stats();
            _system.SetBase(stats, (int)EntityStatType.Strength, 7f);

            _system.SetBase(stats, (int)EntityStatType.Strength, 3f);

            Assert.AreEqual(3f, _system.GetValue(stats, (int)EntityStatType.Strength));
        }

        [Test]
        public void AddBase_from_unset_returns_delta()
        {
            var stats = new Stats();

            float result = _system.AddBase(stats, (int)EntityStatType.Dexterity, 5f);

            Assert.AreEqual(5f, result);
            Assert.AreEqual(5f, _system.GetValue(stats, (int)EntityStatType.Dexterity));
        }

        [Test]
        public void AddBase_accumulates_and_returns_new_value()
        {
            var stats = new Stats();
            _system.SetBase(stats, (int)EntityStatType.Vitality, 10f);

            float result = _system.AddBase(stats, (int)EntityStatType.Vitality, 1f);

            Assert.AreEqual(11f, result);
            Assert.AreEqual(11f, _system.GetValue(stats, (int)EntityStatType.Vitality));
        }

        [Test]
        public void AddBase_preserves_modifiers_on_same_stat()
        {
            var stats = new Stats();
            _system.SetBase(stats, (int)EntityStatType.Strength, 5f);
            _system.AddModifier(stats, new StatModifier((int)EntityStatType.Strength, 10f, ModifierType.Flat, "buff"));

            _system.AddBase(stats, (int)EntityStatType.Strength, 3f);

            // base 5+3=8, plus flat modifier 10 → GetValue 18
            Assert.AreEqual(18f, _system.GetValue(stats, (int)EntityStatType.Strength));
        }

        [Test]
        public void AddUnspent_accumulates()
        {
            var stats = new Stats();

            _system.AddUnspent(stats, 3);
            _system.AddUnspent(stats, 2);

            Assert.AreEqual(5, stats.UnspentPoints);
        }

        [Test]
        public void SetUnspent_overwrites()
        {
            var stats = new Stats();
            _system.AddUnspent(stats, 3);

            _system.SetUnspent(stats, 10);

            Assert.AreEqual(10, stats.UnspentPoints);
        }

        [Test]
        public void Allocate_with_points_spends_one_raises_base_and_returns_new_value()
        {
            var stats = new Stats();
            _system.SetBase(stats, (int)EntityStatType.Strength, 5f);
            _system.SetUnspent(stats, 2);

            int result = _system.Allocate(stats, (int)EntityStatType.Strength);

            Assert.AreEqual(6, result);
            Assert.AreEqual(6f, _system.GetValue(stats, (int)EntityStatType.Strength));
            Assert.AreEqual(1, stats.UnspentPoints);
        }

        [Test]
        public void Allocate_with_zero_points_is_noop_and_returns_current()
        {
            var stats = new Stats();
            _system.SetBase(stats, (int)EntityStatType.Strength, 5f);
            _system.SetUnspent(stats, 0);

            int result = _system.Allocate(stats, (int)EntityStatType.Strength);

            Assert.AreEqual(5, result);
            Assert.AreEqual(5f, _system.GetValue(stats, (int)EntityStatType.Strength));
            Assert.AreEqual(0, stats.UnspentPoints);
        }
    }
}
