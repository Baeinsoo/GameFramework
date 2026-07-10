using NUnit.Framework;
using System;

namespace GameFramework.World.Tests
{
    public class WorldEventBufferTests
    {
        [Test]
        public void Empty_buffer_has_Count_zero_and_empty_snapshot()
        {
            var buffer = new WorldEventBuffer();

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(0, buffer.Snapshot.Count);
        }

        [Test]
        public void Append_increases_Count_and_appears_in_snapshot()
        {
            var buffer = new WorldEventBuffer();
            var e = new DamageDealtEvent("1", "2", 50, false, false);

            buffer.Append(e);

            Assert.AreEqual(1, buffer.Count);
            Assert.AreEqual(1, buffer.Snapshot.Count);
            Assert.AreSame(e, buffer.Snapshot[0]);
        }

        [Test]
        public void Multiple_Appends_preserve_insertion_order()
        {
            var buffer = new WorldEventBuffer();
            var a = new DamageDealtEvent("1", "2", 10, false, false);
            var b = new DeathEvent("3", "2");
            var c = new DamageDealtEvent("3", "2", 20, false, false);

            buffer.Append(a);
            buffer.Append(b);
            buffer.Append(c);

            Assert.AreEqual(3, buffer.Count);
            Assert.AreSame(a, buffer.Snapshot[0]);
            Assert.AreSame(b, buffer.Snapshot[1]);
            Assert.AreSame(c, buffer.Snapshot[2]);
        }

        [Test]
        public void Clear_empties_the_buffer()
        {
            var buffer = new WorldEventBuffer();
            buffer.Append(new DamageDealtEvent("1", "2", 50, false, false));
            buffer.Append(new DeathEvent("1", "2"));

            buffer.Clear();

            Assert.AreEqual(0, buffer.Count);
            Assert.AreEqual(0, buffer.Snapshot.Count);
        }

        [Test]
        public void Append_after_Clear_works_normally()
        {
            var buffer = new WorldEventBuffer();
            buffer.Append(new DeathEvent("1", "2"));
            buffer.Clear();
            var e = new DamageDealtEvent("4", "5", 30, false, false);

            buffer.Append(e);

            Assert.AreEqual(1, buffer.Count);
            Assert.AreSame(e, buffer.Snapshot[0]);
        }

        [Test]
        public void Append_null_throws_ArgumentNullException()
        {
            var buffer = new WorldEventBuffer();

            Assert.Throws<ArgumentNullException>(() => buffer.Append(null));
        }

        [Test]
        public void Append_during_Suppress_is_ignored()
        {
            var buffer = new WorldEventBuffer();

            using (buffer.Suppress())
            {
                buffer.Append(new DeathEvent("1", "2"));
            }

            Assert.AreEqual(0, buffer.Count);
        }

        [Test]
        public void Append_after_Suppress_scope_works_normally()
        {
            var buffer = new WorldEventBuffer();

            using (buffer.Suppress()) { buffer.Append(new DeathEvent("1", "2")); }
            var e = new DamageDealtEvent("3", "4", 10, false, false);
            buffer.Append(e);

            Assert.AreEqual(1, buffer.Count);
            Assert.AreSame(e, buffer.Snapshot[0]);
        }

        [Test]
        public void Events_appended_before_Suppress_are_preserved()
        {
            var buffer = new WorldEventBuffer();
            var a = new DamageDealtEvent("1", "2", 10, false, false);

            buffer.Append(a);
            using (buffer.Suppress()) { buffer.Append(new DeathEvent("9", "2")); }

            Assert.AreEqual(1, buffer.Count);
            Assert.AreSame(a, buffer.Snapshot[0]);
        }

        [Test]
        public void Suppress_is_released_even_if_scope_throws()
        {
            var buffer = new WorldEventBuffer();

            try
            {
                using (buffer.Suppress())
                {
                    buffer.Append(new DeathEvent("1", "2"));   // 무시됨
                    throw new InvalidOperationException("boom");
                }
            }
            catch (InvalidOperationException) { }

            var e = new DamageDealtEvent("3", "4", 5, false, false);
            buffer.Append(e);                                   // 해제됐으니 정상
            Assert.AreEqual(1, buffer.Count);
            Assert.AreSame(e, buffer.Snapshot[0]);
        }

        [Test]
        public void Nested_Suppress_stays_active_until_all_scopes_dispose()
        {
            var buffer = new WorldEventBuffer();

            var outer = buffer.Suppress();
            var inner = buffer.Suppress();
            inner.Dispose();
            buffer.Append(new DeathEvent("1", "2"));            // 아직 outer가 억제 중
            Assert.AreEqual(0, buffer.Count);

            outer.Dispose();
            var e = new DamageDealtEvent("3", "4", 5, false, false);
            buffer.Append(e);                                   // 모두 해제 → 정상
            Assert.AreEqual(1, buffer.Count);
            Assert.AreSame(e, buffer.Snapshot[0]);
        }

        [Test]
        public void Disposing_same_Suppress_scope_twice_does_not_double_decrement()
        {
            var buffer = new WorldEventBuffer();

            var outer = buffer.Suppress();
            var inner = buffer.Suppress();
            inner.Dispose();
            inner.Dispose();                                    // 두 번째는 무시돼야 함
            buffer.Append(new DeathEvent("1", "2"));            // outer가 여전히 억제 중
            Assert.AreEqual(0, buffer.Count);

            outer.Dispose();
            buffer.Append(new DamageDealtEvent("3", "4", 5, false, false));
            Assert.AreEqual(1, buffer.Count);
        }
    }
}
