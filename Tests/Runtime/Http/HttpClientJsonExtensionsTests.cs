using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GameFramework.Http;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GameFramework.Tests.Http
{
    public class HttpClientJsonExtensionsTests
    {
        private class Payload
        {
            public int code;
            public string name;
        }

        [UnityTest]
        public IEnumerator 성공이면_본문을_역직렬화해_돌려준다() => UniTask.ToCoroutine(async () =>
        {
            var client = new HttpClient(FakeHttpMessageHandler.Returning(200, "{\"code\":0,\"name\":\"kim\"}"));

            Payload payload = await client.SendAsync<Payload>(HttpRequestMessage.Get("http://example.com"));

            Assert.That(payload.code, Is.EqualTo(0));
            Assert.That(payload.name, Is.EqualTo("kim"));
        });

        [UnityTest]
        public IEnumerator 커스텀_역직렬화를_주면_그것을_쓴다() => UniTask.ToCoroutine(async () =>
        {
            var client = new HttpClient(FakeHttpMessageHandler.Returning(200, "무엇이든"));

            Payload payload = await client.SendAsync(
                HttpRequestMessage.Get("http://example.com"),
                _ => new Payload { code = 42, name = "custom" });

            Assert.That(payload.code, Is.EqualTo(42));
            Assert.That(payload.name, Is.EqualTo("custom"));
        });

        [UnityTest]
        public IEnumerator _401이면_상태코드를_담은_예외를_던진다() => UniTask.ToCoroutine(async () =>
        {
            var client = new HttpClient(FakeHttpMessageHandler.Returning(401, "{\"message\":\"denied\"}"));

            try
            {
                await client.SendAsync<Payload>(HttpRequestMessage.Get("http://example.com"));
                Assert.Fail("HttpRequestException이 나와야 한다.");
            }
            catch (HttpRequestException exception)
            {
                Assert.That(exception.StatusCode, Is.EqualTo(401));
                Assert.That(exception.ResponseBody, Is.EqualTo("{\"message\":\"denied\"}"));
            }
        });

        [UnityTest]
        public IEnumerator _500도_같은_모양으로_던진다() => UniTask.ToCoroutine(async () =>
        {
            var client = new HttpClient(FakeHttpMessageHandler.Returning(500, "boom"));

            try
            {
                await client.SendAsync<Payload>(HttpRequestMessage.Get("http://example.com"));
                Assert.Fail("HttpRequestException이 나와야 한다.");
            }
            catch (HttpRequestException exception)
            {
                Assert.That(exception.StatusCode, Is.EqualTo(500));
            }
        });

        [UnityTest]
        public IEnumerator 전송이_실패하면_상태코드가_null이다() => UniTask.ToCoroutine(async () =>
        {
            //  이 구분이 이 계층을 새로 짓는 이유다. 예전엔 오프라인과 401이 똑같이 보여서,
            //  오프라인으로 게임을 켠 플레이어의 계정을 "서버가 거부했다"로 오판해 지웠다.
            var client = new HttpClient(FakeHttpMessageHandler.Throwing(new HttpRequestException("연결 실패")));

            try
            {
                await client.SendAsync<Payload>(HttpRequestMessage.Get("http://example.com"));
                Assert.Fail("HttpRequestException이 나와야 한다.");
            }
            catch (HttpRequestException exception)
            {
                Assert.That(exception.StatusCode, Is.Null);
            }
        });
    }
}
