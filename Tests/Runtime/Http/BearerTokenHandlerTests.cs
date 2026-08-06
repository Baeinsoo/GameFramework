using System.Collections;
using Cysharp.Threading.Tasks;
using GameFramework.Http;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GameFramework.Tests.Http
{
    public class BearerTokenHandlerTests
    {
        [UnityTest]
        public IEnumerator 토큰이_있으면_Authorization을_붙인다() => UniTask.ToCoroutine(async () =>
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var client = new HttpClient(new BearerTokenHandler(fake, () => "abc.def.ghi"));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests[0].Headers["Authorization"], Is.EqualTo("Bearer abc.def.ghi"));
        });

        [UnityTest]
        public IEnumerator 토큰이_없으면_아무것도_붙이지_않는다() => UniTask.ToCoroutine(async () =>
        {
            //  로그인 전에는 토큰이 없다 — 빈 Bearer를 보내면 서버가 잘못된 토큰으로 읽는다.
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var client = new HttpClient(new BearerTokenHandler(fake, () => null));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests[0].Headers.ContainsKey("Authorization"), Is.False);
        });
    }
}
