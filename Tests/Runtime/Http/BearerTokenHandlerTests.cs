using System.Collections;
using System.Collections.Generic;
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
            var client = new HttpClient(new BearerTokenHandler(fake, FakeAccessTokenProvider.Returning("abc.def.ghi")));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests[0].Headers["Authorization"], Is.EqualTo("Bearer abc.def.ghi"));
        });

        [UnityTest]
        public IEnumerator 토큰이_없으면_아무것도_붙이지_않는다() => UniTask.ToCoroutine(async () =>
        {
            //  로그인 전에는 토큰이 없다 — 빈 Bearer를 보내면 서버가 잘못된 토큰으로 읽는다.
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var client = new HttpClient(new BearerTokenHandler(fake, FakeAccessTokenProvider.Returning(null)));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests[0].Headers.ContainsKey("Authorization"), Is.False);
        });

        [UnityTest]
        public IEnumerator 성공하면_갱신을_부르지_않는다() => UniTask.ToCoroutine(async () =>
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var provider = FakeAccessTokenProvider.Returning("old");
            var client = new HttpClient(new BearerTokenHandler(fake, provider));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(provider.Calls, Is.EqualTo(new[] { false }));
            Assert.That(fake.Requests.Count, Is.EqualTo(1));
        });

        [UnityTest]
        public IEnumerator _401이면_갱신해서_한_번_다시_보낸다() => UniTask.ToCoroutine(async () =>
        {
            var sent = new List<string>();

            //  보낼 때의 헤더를 그 자리에서 남긴다 — 재전송은 같은 요청 객체를 다시 쓰므로
            //  나중에 Requests에서 읽으면 최종값 하나만 보인다.
            var fake = new FakeHttpMessageHandler((request, _) =>
            {
                sent.Add(request.Headers.TryGetValue("Authorization", out string value) ? value : null);
                return UniTask.FromResult(new HttpResponseMessage(sent.Count == 1 ? 401 : 200, "{}"));
            });

            var provider = new FakeAccessTokenProvider(forceRefresh => forceRefresh ? "new" : "old");
            var client = new HttpClient(new BearerTokenHandler(fake, provider));

            HttpResponseMessage response = await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(sent, Is.EqualTo(new[] { "Bearer old", "Bearer new" }));
            Assert.That(provider.Calls, Is.EqualTo(new[] { false, true }));
            Assert.That(response.StatusCode, Is.EqualTo(200));
        });

        [UnityTest]
        public IEnumerator 갱신해도_토큰이_그대로면_다시_보내지_않는다() => UniTask.ToCoroutine(async () =>
        {
            //  갱신이 실패하면 공급자는 지금 가진 토큰을 그대로 준다. 방금 거부당한 토큰을 다시
            //  보내봐야 결과가 같으므로 헛수고를 하지 않는다.
            var fake = FakeHttpMessageHandler.Returning(401, "{}");
            var provider = FakeAccessTokenProvider.Returning("same");
            var client = new HttpClient(new BearerTokenHandler(fake, provider));

            HttpResponseMessage response = await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests.Count, Is.EqualTo(1));
            Assert.That(response.StatusCode, Is.EqualTo(401));
        });

        [UnityTest]
        public IEnumerator 로그인_상태가_아니면_다시_보내지_않는다() => UniTask.ToCoroutine(async () =>
        {
            var fake = FakeHttpMessageHandler.Returning(401, "{}");
            var provider = FakeAccessTokenProvider.Returning(null);
            var client = new HttpClient(new BearerTokenHandler(fake, provider));

            HttpResponseMessage response = await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests.Count, Is.EqualTo(1));
            Assert.That(response.StatusCode, Is.EqualTo(401));
        });

        [UnityTest]
        public IEnumerator 다시_보낸_것도_401이면_그대로_돌려준다() => UniTask.ToCoroutine(async () =>
        {
            //  재전송은 루프가 아니라 한 번뿐이다 — 여기서 안 멈추면 401이 무한히 반복된다.
            var fake = FakeHttpMessageHandler.Returning(401, "{}");
            var provider = new FakeAccessTokenProvider(forceRefresh => forceRefresh ? "new" : "old");
            var client = new HttpClient(new BearerTokenHandler(fake, provider));

            HttpResponseMessage response = await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests.Count, Is.EqualTo(2));
            Assert.That(provider.Calls, Is.EqualTo(new[] { false, true }));
            Assert.That(response.StatusCode, Is.EqualTo(401));
        });
    }
}
