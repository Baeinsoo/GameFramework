using System.Numerics;
using GameFramework.Netcode;
using NUnit.Framework;

namespace GameFramework.Tests.Netcode
{
    public class ReconcileGateTests
    {
        [Test]
        public void WithinThreshold_DoesNotReconcile()
        {
            var a = new Vector3(0, 0, 0);
            var b = new Vector3(0.05f, 0, 0);
            Assert.IsFalse(ReconcileGate.ShouldReconcile(a, b, 0.06f));
        }

        [Test]
        public void BeyondThreshold_Reconciles()
        {
            var a = new Vector3(0, 0, 0);
            var b = new Vector3(0.2f, 0, 0);
            Assert.IsTrue(ReconcileGate.ShouldReconcile(a, b, 0.06f));
        }

        [Test]
        public void ExactlyAtThreshold_DoesNotReconcile()
        {
            var a = new Vector3(0, 0, 0);
            var b = new Vector3(0.06f, 0, 0);
            Assert.IsFalse(ReconcileGate.ShouldReconcile(a, b, 0.06f));
        }
    }
}
