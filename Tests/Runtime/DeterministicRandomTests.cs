using NUnit.Framework;

namespace GameFramework.Tests
{
    public class DeterministicRandomTests
    {
        [Test]
        public void Same_seed_produces_identical_sequence()
        {
            var a = new DeterministicRandom(12345UL);
            var b = new DeterministicRandom(12345UL);

            for (int i = 0; i < 100; i++)
            {
                Assert.AreEqual(a.NextUInt64(), b.NextUInt64(), $"mismatch at draw {i}");
            }
        }

        [Test]
        public void Different_seeds_diverge()
        {
            var a = new DeterministicRandom(1UL);
            var b = new DeterministicRandom(2UL);

            Assert.AreNotEqual(a.NextUInt64(), b.NextUInt64());
        }

        // 알고리즘 고정(회귀 잠금): seed=0의 SplitMix64 표준 레퍼런스 벡터.
        // 실패 시 기대값을 impl에 맞추지 말고 구현을 고쳐라.
        [Test]
        public void Seed_zero_matches_splitmix64_reference_vectors()
        {
            var r = new DeterministicRandom(0UL);

            Assert.AreEqual(0xE220A8397B1DCDAFUL, r.NextUInt64());
            Assert.AreEqual(0x6E789E6AA1B965F4UL, r.NextUInt64());
            Assert.AreEqual(0x06C45D188009454FUL, r.NextUInt64());
            Assert.AreEqual(0xF88BB8A8724C81ECUL, r.NextUInt64());
            Assert.AreEqual(0x1B39896A51A8749BUL, r.NextUInt64());
        }

        [Test]
        public void NextFloat01_is_in_unit_interval()
        {
            var r = new DeterministicRandom(999UL);
            for (int i = 0; i < 10000; i++)
            {
                float f = r.NextFloat01();
                Assert.GreaterOrEqual(f, 0f);
                Assert.Less(f, 1f);
            }
        }

        [Test]
        public void Range_float_is_in_half_open_interval()
        {
            var r = new DeterministicRandom(7UL);
            for (int i = 0; i < 10000; i++)
            {
                float f = r.Range(1.25f, 1.75f);
                Assert.GreaterOrEqual(f, 1.25f);
                Assert.Less(f, 1.75f);
            }
        }

        [Test]
        public void Range_float_equal_bounds_returns_min()
        {
            var r = new DeterministicRandom(7UL);
            Assert.AreEqual(3f, r.Range(3f, 3f));
        }

        [Test]
        public void Range_int_is_in_half_open_interval_and_covers_ends()
        {
            var r = new DeterministicRandom(42UL);
            bool sawMin = false, sawMax = false;
            for (int i = 0; i < 10000; i++)
            {
                int v = r.Range(2, 5); // {2,3,4}
                Assert.GreaterOrEqual(v, 2);
                Assert.Less(v, 5);
                if (v == 2) sawMin = true;
                if (v == 4) sawMax = true;
            }
            Assert.IsTrue(sawMin, "never produced min");
            Assert.IsTrue(sawMax, "never produced max-1");
        }

        [Test]
        public void Range_int_empty_or_inverted_returns_min()
        {
            var r = new DeterministicRandom(42UL);
            Assert.AreEqual(5, r.Range(5, 5));   // empty
            Assert.AreEqual(9, r.Range(9, 3));   // inverted (max<min) → min, no-op
        }
    }
}
