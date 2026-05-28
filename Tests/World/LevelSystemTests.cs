using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class LevelSystemTests
    {
        private readonly LevelSystem _system = new LevelSystem();

        private static Level NewLevel(int value = 1, long exp = 0, long expToNext = 100)
            => new Level { Value = value, Exp = exp, ExpToNext = expToNext };

        [Test]
        public void AddExperience_below_threshold_increases_exp_without_leveling()
        {
            var level = NewLevel();

            _system.AddExperience(level, 50);

            Assert.AreEqual(1, level.Value);
            Assert.AreEqual(50, level.Exp);
        }

        [Test]
        public void AddExperience_reaching_threshold_levels_up_and_carries_remainder()
        {
            var level = NewLevel();

            _system.AddExperience(level, 120);

            Assert.AreEqual(2, level.Value);
            Assert.AreEqual(20, level.Exp);
        }

        [Test]
        public void AddExperience_exactly_threshold_levels_up_with_zero_remainder()
        {
            var level = NewLevel();

            _system.AddExperience(level, 100);

            Assert.AreEqual(2, level.Value);
            Assert.AreEqual(0, level.Exp);
        }

        [Test]
        public void AddExperience_spanning_multiple_levels()
        {
            var level = NewLevel();

            _system.AddExperience(level, 250);

            Assert.AreEqual(3, level.Value);
            Assert.AreEqual(50, level.Exp);
        }

        [Test]
        public void AddExperience_does_not_level_up_when_threshold_is_not_positive()
        {
            var level = NewLevel(value: 1, exp: 0, expToNext: 0);

            _system.AddExperience(level, 50);

            Assert.AreEqual(1, level.Value);
            Assert.AreEqual(50, level.Exp);
        }
    }
}
