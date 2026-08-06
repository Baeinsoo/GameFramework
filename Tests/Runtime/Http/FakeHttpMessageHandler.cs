using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using GameFramework.Http;

namespace GameFramework.Tests.Http
{
    /// <summary>체인의 끝을 대신하는 가짜 전송. 네트워크 없이 응답을 정해줄 수 있다.</summary>
    public sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, UniTask<HttpResponseMessage>> onSend;

        public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();
        public CancellationToken LastCancellationToken { get; private set; }

        //  토큰까지 받는 이유: 타임아웃 검증이 "HttpClient가 넘겨준 토큰이 실제로 취소되는가"라서,
        //  가짜 핸들러도 그 토큰을 존중해야 의미가 있다.
        public FakeHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, UniTask<HttpResponseMessage>> onSend)
        {
            this.onSend = onSend;
        }

        public static FakeHttpMessageHandler Returning(long statusCode, string body)
        {
            return new FakeHttpMessageHandler((_, __) => UniTask.FromResult(new HttpResponseMessage(statusCode, body)));
        }

        public static FakeHttpMessageHandler Throwing(Exception exception)
        {
            return new FakeHttpMessageHandler((_, __) => UniTask.FromException<HttpResponseMessage>(exception));
        }

        //  여기서 취소를 검사하지 않는다 — 검사하면 "HttpClient가 취소를 막았다"와 "가짜가 막았다"를
        //  구분할 수 없어져, 취소 테스트가 HttpClient를 실제로 검증하지 못한다.
        public override UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastCancellationToken = cancellationToken;
            Requests.Add(request);

            return onSend(request, cancellationToken);
        }
    }
}
