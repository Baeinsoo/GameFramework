using GameFramework.Auth;
using NUnit.Framework;

namespace GameFramework.Tests.Auth
{
    public class AuthProfileTests
    {
        [Test]
        public void 인자가_없으면_기본_프로필이다()
        {
            Assert.AreEqual(AuthProfile.DefaultProfile, AuthProfile.Resolve(new[] { "Unity.exe" }));
        }

        //  MPPM의 첫 인스턴스는 Player1이며 그것을 기본과 같은 계정으로 본다 —
        //  그렇지 않으면 평소 에디터 실행과 첫 인스턴스가 서로 다른 계정이 되어 혼란스럽다.
        [Test]
        public void Player1은_기본_프로필과_같다()
        {
            Assert.AreEqual(AuthProfile.DefaultProfile, AuthProfile.Resolve(new[] { "Unity.exe", "-name", "Player1" }));
        }

        [Test]
        public void 다른_인스턴스_이름은_그대로_프로필이_된다()
        {
            Assert.AreEqual("Player2", AuthProfile.Resolve(new[] { "Unity.exe", "-name", "Player2" }));
        }

        [Test]
        public void 값이_빠진_인자는_기본_프로필이다()
        {
            Assert.AreEqual(AuthProfile.DefaultProfile, AuthProfile.Resolve(new[] { "Unity.exe", "-name" }));
        }

        [Test]
        public void 인자가_null이어도_죽지_않는다()
        {
            Assert.AreEqual(AuthProfile.DefaultProfile, AuthProfile.Resolve(null));
        }
    }
}
