using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    /// <summary>핸들러 체인의 진입점. 4xx·5xx여도 예외를 던지지 않고 응답을 그대로 돌려준다 —
    /// 상태코드를 보고 재시도할지 정하는 것은 체인 안의 핸들러 몫이고, 예외로 바꾸는 것은
    /// 그 위 타입드 계층(SendAsync&lt;T&gt;) 몫이다.</summary>
    public class HttpClient
    {
        public static readonly TimeSpan InfiniteTimeout = System.Threading.Timeout.InfiniteTimeSpan;

        private readonly HttpMessageHandler handler;

        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

        public HttpClient(HttpMessageHandler handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public async UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            //  호출자 토큰과 타임아웃을 하나로 묶어 넘긴다 — 둘 중 어느 쪽이 먼저 와도 끊긴다.
            using (var timeoutSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
            {
                if (Timeout != InfiniteTimeout)
                {
                    timeoutSource.CancelAfter(Timeout);
                }

                return await handler.SendAsync(request, timeoutSource.Token);
            }
        }
    }
}
