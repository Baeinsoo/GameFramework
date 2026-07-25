using NUnit.Framework;

namespace GameFramework.World.Tests
{
    public class GroundStateTests
    {
        [Test]
        public void DefaultsToNotGrounded()
        {
            var state = new GroundState();
            Assert.IsFalse(state.IsGrounded);
        }

        [Test]
        public void AttachesToEntityAndRoundTrips()
        {
            var entity = new Entity("e1");
            entity.Add(new GroundState { IsGrounded = true });

            Assert.IsTrue(entity.Get<GroundState>().IsGrounded);
            Assert.AreSame(entity, entity.Get<GroundState>().Owner);
        }
    }
}
