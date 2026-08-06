using GameFramework.Http;
using NUnit.Framework;

namespace GameFramework.Tests.Http
{
    public class HttpMessageTypesTests
    {
        [Test]
        public void Put_본문이_있으면_JSON으로_직렬화하고_컨텐트타입을_붙인다()
        {
            var request = HttpRequestMessage.Put("http://example.com/lobby", new { userId = "abc" });

            Assert.That(request.Method, Is.EqualTo(GameFramework.Http.HttpMethod.PUT));
            Assert.That(request.Uri, Is.EqualTo("http://example.com/lobby"));
            Assert.That(request.Content, Does.Contain("abc"));
            Assert.That(request.ContentType, Is.EqualTo("application/json"));
        }

        [Test]
        public void Get_은_본문이_없다()
        {
            var request = HttpRequestMessage.Get("http://example.com/user/1");

            Assert.That(request.Method, Is.EqualTo(GameFramework.Http.HttpMethod.GET));
            Assert.That(request.Content, Is.Null);
            Assert.That(request.Headers, Is.Empty);
        }

        [Test]
        [TestCase(200, true)]
        [TestCase(299, true)]
        [TestCase(300, false)]
        [TestCase(401, false)]
        [TestCase(500, false)]
        public void IsSuccessStatusCode_는_2xx만_참이다(long statusCode, bool expected)
        {
            var response = new HttpResponseMessage(statusCode, string.Empty);

            Assert.That(response.IsSuccessStatusCode, Is.EqualTo(expected));
        }

        [Test]
        public void EnsureSuccessStatusCode_는_2xx가_아니면_상태코드와_본문을_담아_던진다()
        {
            var response = new HttpResponseMessage(401, "{\"message\":\"denied\"}");

            var exception = Assert.Throws<HttpRequestException>(() => response.EnsureSuccessStatusCode());

            Assert.That(exception.StatusCode, Is.EqualTo(401));
            Assert.That(exception.ResponseBody, Is.EqualTo("{\"message\":\"denied\"}"));
        }

        [Test]
        public void 전송_실패용_생성자는_상태코드가_null이다()
        {
            //  이 null이 "서버에 닿지도 못했다"의 유일한 신호다 — 401(서버가 거부)과 반드시 구분돼야
            //  한다. 예전에 이 둘을 뭉개서 오프라인 플레이어의 계정을 지우는 사고가 났다.
            var exception = new HttpRequestException("연결 실패");

            Assert.That(exception.StatusCode, Is.Null);
        }
    }
}
