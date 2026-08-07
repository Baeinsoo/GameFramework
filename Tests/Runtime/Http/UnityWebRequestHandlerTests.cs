using System.Collections;
using Cysharp.Threading.Tasks;
using GameFramework.Http;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace GameFramework.Tests.Http
{
    public class UnityWebRequestHandlerTests
    {
        [UnityTest]
        public IEnumerator 연결_자체가_안되면_상태코드_없는_예외를_던진다() => UniTask.ToCoroutine(async () =>
        {
            //  이 테스트는 원래 브리프엔 없던 추가 요구사항이다: "서버가 거절했다(상태코드 있음)"와
            //  "서버에 아예 닿지 못했다(상태코드 없음)"를 혼동하면, 오프라인 상태를 로그인 거절로
            //  잘못 읽어 계정을 삭제하는 사고로 이어질 수 있다. 그래서 진짜 UnityWebRequestHandler로,
            //  아무도 듣고 있지 않은 포트에 실제로 연결을 시도해 그 구분이 지켜지는지 확인한다.
            var httpClient = new HttpClient(new UnityWebRequestHandler()) { Timeout = System.TimeSpan.FromSeconds(5) };

            try
            {
                await httpClient.SendAsync(HttpRequestMessage.Get("http://127.0.0.1:59999/"));
                Assert.Fail("아무도 듣지 않는 포트인데 요청이 성공했다.");
            }
            catch (HttpRequestException exception)
            {
                Assert.That(exception.StatusCode, Is.Null,
                    "연결 실패는 서버 거부(상태코드 있음)와 반드시 구분되어야 한다.");
            }
        });
    }
}
