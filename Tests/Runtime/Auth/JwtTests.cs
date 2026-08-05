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
            string payload = $"{{\"sub\":\"{subject}\",\"exp\":{expUnixSeconds}}}";
            return MakeTokenWithHeaderAndPayload("{\"alg\":\"HS256\",\"typ\":\"JWT\"}", payload, secret);
        }

        //  헤더/페이로드를 자유롭게 바꿔 넣고, 서명은 항상 진짜 HS256 HMAC으로 계산한다.
        //  "서명은 맞는데 헤더만 다른" 토큰을 만들어야 alg 체크 하나만 격리해서 검증할 수 있다.
        private static string MakeTokenWithHeaderAndPayload(string headerJson, string payloadJson, string secret)
        {
            string header = Base64Url(Encoding.UTF8.GetBytes(headerJson));
            string payload = Base64Url(Encoding.UTF8.GetBytes(payloadJson));
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

        //  서명은 진짜 HS256 HMAC이라 서명 검증은 통과한다 — 오직 alg 체크만 이 토큰을 막을 수 있다.
        //  alg 체크를 지워도 서명 계산이 항상 HS256이라 통과해 버리는 걸 이 테스트가 잡아낸다.
        [Test]
        public void alg가_다르면_서명이_맞아도_실패한다()
        {
            string payload = $"{{\"sub\":\"user-1\",\"exp\":{Now.ToUnixTimeSeconds() + 3600}}}";
            string token = MakeTokenWithHeaderAndPayload("{\"alg\":\"HS384\",\"typ\":\"JWT\"}", payload, Secret);

            Assert.IsFalse(Jwt.TryVerifyHs256(token, Secret, out _, Now));
        }

        //  alg를 none으로 적고 서명 칸을 비운 고전적 우회 모양. 이건 alg 체크가 아니라
        //  "서명 칸이 비어 계산값과 다르다"는 이유로 거부된다 — 이름을 그에 맞게 붙인다.
        [Test]
        public void 서명이_비어있으면_실패한다()
        {
            string header = Base64Url(Encoding.UTF8.GetBytes("{\"alg\":\"none\",\"typ\":\"JWT\"}"));
            string payload = Base64Url(Encoding.UTF8.GetBytes($"{{\"sub\":\"user-1\",\"exp\":{Now.ToUnixTimeSeconds() + 3600}}}"));

            Assert.IsFalse(Jwt.TryVerifyHs256($"{header}.{payload}.", Secret, out _, Now));
        }

        //  sub 안에 이스케이프된 따옴표(\")가 있으면 파서가 진짜 닫는 따옴표를 못 찾고 엉뚱한
        //  위치에서 잘라낸다 — 값을 추측해 돌려주지 말고 거부해야 한다(잘못된 신원 인정 방지).
        [Test]
        public void sub에_이스케이프된_따옴표가_있으면_거부한다()
        {
            string payload = "{\"sub\":\"user\\\"1\",\"exp\":" + (Now.ToUnixTimeSeconds() + 3600) + "}";
            string token = MakeTokenWithHeaderAndPayload("{\"alg\":\"HS256\",\"typ\":\"JWT\"}", payload, Secret);

            Assert.IsFalse(Jwt.TryVerifyHs256(token, Secret, out string subject, Now));
            Assert.IsNull(subject);
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
