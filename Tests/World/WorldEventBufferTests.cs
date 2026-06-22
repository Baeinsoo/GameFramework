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
    }
}
