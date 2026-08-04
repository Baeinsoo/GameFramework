using System;
using System.Security.Cryptography;
using System.Text;
using GameFramework.Auth;
using NUnit.Framework;

namespace GameFramework.Tests.Auth
{
    public class JwtTests
    {
        private const string Secret = "test-secret-0123456789";
        private static readonly DateTimeOffset Now = DateTimeOffset.FromUnixTimeSeconds(1_800_000_000);

        //  서버(jsonwebtoken)가 만드는 것과 같은 모양의 토큰을 테스트 안에서 직접 만든다.
        //  구현이 자기 자신이 만든 토큰만 통과시키는 상황을 피하려면 인코딩을 독립적으로 재현해야 한다.
        private static string MakeToken(string subject, long expUnixSeconds, string secret)
        {
            string header = Base64Url(Encoding.UTF8.GetBytes("{\"alg\":\"HS256\",\"typ\":\"JWT\"}"));
            string payload = Base64Url(Encoding.UTF8.GetBytes($"{{\"sub\":\"{subject}\",\"exp\":{expUnixSeconds}}}"));
            string signingInput = $"{header}.{payload}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            string signature = Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));

            return $"{signingInput}.{signature}";
        }

        private static string Base64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        [Test]
        public void 유효한_토큰이면_sub를_돌려준다()
        {
            string token = MakeToken("user-1", Now.ToUnixTimeSeconds() + 3600, Secret);

            Assert.IsTrue(Jwt.TryVerifyHs256(token, Secret, out string subject, Now));
            Assert.AreEqual("user-1", subject);
        }

        [Test]
        public void 다른_키로_검증하면_실패한다()
        {
            string token = MakeToken("user-1", Now.ToUnixTimeSeconds() + 3600, Secret);

            Assert.IsFalse(Jwt.TryVerifyHs256(token, "another-secret", out string subject, Now));
            Assert.IsNull(subject);
        }

        //  서명이 깨진 토큰은 "이상한 값"이 아니라 위조 시도다. 절대 통과하면 안 된다.
        [Test]
        public void 페이로드를_바꿔치기하면_실패한다()
        {
            string token = MakeToken("user-1", Now.ToUnixTimeSeconds() + 3600, Secret);
            string[] parts = token.Split('.');
            string forged = Base64Url(Encoding.UTF8.GetBytes("{\"sub\":\"user-2\",\"exp\":9999999999}"));

            Assert.IsFalse(Jwt.TryVerifyHs256($"{parts[0]}.{forged}.{parts[2]}", Secret, out _, Now));
        }

        [Test]
        public void 만료된_토큰은_실패한다()
        {
            string token = MakeToken("user-1", Now.ToUnixTimeSeconds() - 1, Secret);

            Assert.IsFalse(Jwt.TryVerifyHs256(token, Secret, out _, Now));
        }

        [Test]
        public void 만료_직전은_통과한다()
        {
            string token = MakeToken("user-1", Now.ToUnixTimeSeconds() + 1, Secret);

            Assert.IsTrue(Jwt.TryVerifyHs256(token, Secret, out _, Now));
        }

        //  alg를 none으로 바꾸고 서명을 비운 고전적 우회. 알고리즘을 고정하지 않으면 통과해 버린다.
        [Test]
        public void alg_none_토큰은_실패한다()
        {
            string header = Base64Url(Encoding.UTF8.GetBytes("{\"alg\":\"none\",\"typ\":\"JWT\"}"));
            string payload = Base64Url(Encoding.UTF8.GetBytes($"{{\"sub\":\"user-1\",\"exp\":{Now.ToUnixTimeSeconds() + 3600}}}"));

            Assert.IsFalse(Jwt.TryVerifyHs256($"{header}.{payload}.", Secret, out _, Now));
        }

        [TestCase("")]
        [TestCase("not-a-token")]
        [TestCase("a.b")]
        [TestCase("a.b.c.d")]
        public void 형식이_아니면_실패한다(string token)
        {
            Assert.IsFalse(Jwt.TryVerifyHs256(token, Secret, out _, Now));
        }
    }
}
