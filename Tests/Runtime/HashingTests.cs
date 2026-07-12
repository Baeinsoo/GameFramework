using NUnit.Framework;

namespace GameFramework.Tests
{
    public class HashingTests
    {
        [Test]
        public void Fnv1a64_is_deterministic_for_same_string()
        {
            Assert.AreEqual(Hashing.Fnv1a64("entity-42"), Hashing.Fnv1a64("entity-42"));
        }

        [Test]
        public void Fnv1a64_differs_for_different_strings()
        {
            Assert.AreNotEqual(Hashing.Fnv1a64("a"), Hashing.Fnv1a64("b"));
        }

        [Test]
        public void Fnv1a64_null_is_safe_and_equals_empty()
        {
            Assert.AreEqual(Hashing.Fnv1a64(""), Hashing.Fnv1a64(null));
        }

        [Test]
        public void Combine_is_deterministic()
        {
            Assert.AreEqual(Hashing.Combine(123UL, 456UL), Hashing.Combine(123UL, 456UL));
        }

        [Test]
        public void Combine_is_order_sensitive()
        {
            Assert.AreNotEqual(Hashing.Combine(1UL, 2UL), Hashing.Combine(2UL, 1UL));
        }
    }
}
