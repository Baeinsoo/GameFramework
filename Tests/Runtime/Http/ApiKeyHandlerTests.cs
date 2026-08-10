using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using GameFramework.Http;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GameFramework.Tests.Http
{
    public class ApiKeyHandlerTests
    {
        [UnityTest]
        public IEnumerator 키가_있으면_지정한_헤더에_붙인다() => UniTask.ToCoroutine(async () =>
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var client = new HttpClient(new ApiKeyHandler(fake, "X-Internal-Api-Key", () => "secret-key"));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests[0].Headers["X-Internal-Api-Key"], Is.EqualTo("secret-key"));
        });

        //  빈 키를 보내면 서버가 "틀린 키"로 읽어 401을 준다 — 아예 안 붙이는 편이 낫다.
        [UnityTest]
        public IEnumerator 키가_비어_있으면_아무것도_붙이지_않는다() => UniTask.ToCoroutine(async () =>
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            var client = new HttpClient(new ApiKeyHandler(fake, "X-Internal-Api-Key", () => null));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests[0].Headers.ContainsKey("X-Internal-Api-Key"), Is.False);
        });

        //  환경변수가 프로세스 시작 뒤에 채워질 수 있어, 만들 때 한 번 읽으면 빈 값이 굳는다.
        [UnityTest]
        public IEnumerator 키를_보낼_때마다_다시_읽는다() => UniTask.ToCoroutine(async () =>
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");
            string key = "first";
            var client = new HttpClient(new ApiKeyHandler(fake, "X-Internal-Api-Key", () => key));

            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));
            key = "second";
            await client.SendAsync(HttpRequestMessage.Get("http://example.com"));

            Assert.That(fake.Requests[0].Headers["X-Internal-Api-Key"], Is.EqualTo("first"));
            Assert.That(fake.Requests[1].Headers["X-Internal-Api-Key"], Is.EqualTo("second"));
        });

        [Test]
        public void 키_공급자가_없으면_생성에서_던진다()
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");

            Assert.Throws<ArgumentNullException>(() => new ApiKeyHandler(fake, "X-Internal-Api-Key", null));
        }

        [Test]
        public void 헤더_이름이_비면_생성에서_던진다()
        {
            var fake = FakeHttpMessageHandler.Returning(200, "{}");

            Assert.Throws<ArgumentException>(() => new ApiKeyHandler(fake, "", () => "k"));
        }
    }
}
