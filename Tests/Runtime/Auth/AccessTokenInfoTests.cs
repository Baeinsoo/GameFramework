using System;
using GameFramework.Auth;
using NUnit.Framework;

namespace GameFramework.Tests.Auth
{
    public class AccessTokenInfoTests
    {
        private static readonly DateTimeOffset Issued = DateTimeOffset.FromUnixTimeSeconds(1_800_000_000);

        [Test]
        public void expiresIn_초를_만료시각으로_바꾼다()
        {
            var info = AccessTokenInfo.FromExpiresIn("t", 3600, Issued);

            Assert.AreEqual(Issued.AddSeconds(3600), info.ExpiresAt);
            Assert.AreEqual("t", info.Token);
        }

        [Test]
        public void 만료_전에는_만료가_아니다()
        {
            var info = AccessTokenInfo.FromExpiresIn("t", 3600, Issued);

            Assert.IsFalse(info.IsExpired(Issued.AddSeconds(3599)));
        }

        [Test]
        public void 만료_시각_정각부터_만료다()
        {
            var info = AccessTokenInfo.FromExpiresIn("t", 3600, Issued);

            Assert.IsTrue(info.IsExpired(Issued.AddSeconds(3600)));
        }

        //  갱신은 만료보다 먼저 일어나야 한다 — 만료된 뒤에 갱신하면 그 사이 요청이 401을 맞는다.
        [Test]
        public void 만료_5분_전부터_갱신이_필요하다()
        {
            var info = AccessTokenInfo.FromExpiresIn("t", 3600, Issued);
            var margin = AccessTokenInfo.DefaultRefreshMargin;

            Assert.IsFalse(info.NeedsRefresh(Issued.AddSeconds(3600 - 301), margin));
            Assert.IsTrue(info.NeedsRefresh(Issued.AddSeconds(3600 - 300), margin));
        }

        [Test]
        public void 이미_만료됐으면_갱신도_필요하다()
        {
            var info = AccessTokenInfo.FromExpiresIn("t", 3600, Issued);

            Assert.IsTrue(info.NeedsRefresh(Issued.AddSeconds(7200), AccessTokenInfo.DefaultRefreshMargin));
        }

        [Test]
        public void 기본값은_비어있는_토큰이고_항상_만료다()
        {
            var info = default(AccessTokenInfo);

            Assert.IsTrue(info.IsExpired(Issued));
        }
    }
}
