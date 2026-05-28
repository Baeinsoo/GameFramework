using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class ManaTests
    {
        [Test]
        public void Constructor_sets_Current_to_Max()
        {
            var mana = new Mana(50);

            Assert.AreEqual(50, mana.Current);
        }
    }
}
