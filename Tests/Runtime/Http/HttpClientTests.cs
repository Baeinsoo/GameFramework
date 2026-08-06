using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Http;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GameFramework.Tests.Http
{
    public class HttpClientTests
    {
        //  바깥 핸들러가 안쪽을 감싸는지 순서를 기록으로 확인한다.
        private class RecordingHandler : DelegatingHandler
        {
            private readonly List<string> log;
            private readonly string name;

            public RecordingHandler(HttpMessageHandler inner, List<string> log, string name) : base(inner)
            {
                this.log = log;
                this.name = name;
            }

            public override async UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                log.Add($"{name}:before");
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                log.Add($"{name}:after");
                return response;
            }
        }

        //  슬라이스 1의 401 재시도가 성립하려면 핸들러가 응답을 보고 다시 보낼 수 있어야 한다.
        private class RetryOnceHandler : DelegatingHandler
        {
            public RetryOnceHandler(HttpMessageHandler inner) : base(inner) { }

            public override async UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

                if (response.StatusCode == 401)
                {
                    response = await base.SendAsync(request, cancellationToken);
                }

                return response;
            }
        }

        [UnityTest]
        public IEnumerator 핸들러는_바깥부터_들어가_안쪽부터_나온다() => UniTask.ToCoroutine(async () =>
        {
            var log = new List<string>();
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var client = new HttpClient(new RecordingHandler(new RecordingHandler(fake, log, "inner"), log, "outer"));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(log, Is.EqualTo(new[] { "outer:before", "inner:before", "inner:after", "outer:after" }));
        });

        [UnityTest]
        public IEnumerator 핸들러는_응답을_보고_다시_보낼_수_있다() => UniTask.ToCoroutine(async () =>
        {
            int calls = 0;
            var fake = new FakeHttpMessageHandler((_, __) =>
            {
                calls++;
                return UniTask.FromResult(new HttpResponseMessage(calls == 1 ? 401 : 200, "{}"));
            });
            var client = new HttpClient(new RetryOnceHandler(fake));

            HttpResponseMessage response = await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(calls, Is.EqualTo(2));
            Assert.That(response.StatusCode, Is.EqualTo(200));
        });

        [UnityTest]
        public IEnumerator 이미_취소된_토큰이면_전송하지_않는다() => UniTask.ToCoroutine(async () =>
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var client = new HttpClient(fake);
            var cancelled = new CancellationTokenSource();
            cancelled.Cancel();

            try
            {
                await client.SendAsync(HttpRequestMessage.Get("http://example.com"), cancelled.Token);
                Assert.Fail("OperationCanceledException이 나와야 한다.");
            }
            catch (OperationCanceledException)
            {
            }

            Assert.That(fake.Requests, Is.Empty);
        });

        [UnityTest]
        public IEnumerator 타임아웃이_지나면_취소된다() => UniTask.ToCoroutine(async () =>
        {
            //  핸들러가 스스로는 끝나지 않고, 넘겨받은 토큰이 취소될 때만 끝난다. 그 토큰은
            //  HttpClient가 타임아웃과 묶어서 만든 것이라, 취소가 오면 곧 타임아웃이 동작한 것이다.
            //  (핸들러가 토큰을 무시하면 아무도 못 끊는다 — .NET도 같은 계약이다.)
            var fake = new FakeHttpMessageHandler((_, cancellationToken) => UniTask.Never<HttpResponseMessage>(cancellationToken));
            var client = new HttpClient(fake) { Timeout = TimeSpan.FromMilliseconds(50) };

            try
            {
                await client.SendAsync(HttpRequestMessage.Get("http://example.com"));
                Assert.Fail("OperationCanceledException이 나와야 한다.");
            }
            catch (OperationCanceledException)
            {
            }
        });
    }
}
