using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    /// <summary>공급자가 토큰을 주면 Authorization 헤더를 붙인다. 공급자를 매번 호출하는 이유는
    /// 토큰이 갱신으로 바뀌기 때문 — 만들 때의 값을 스냅샷으로 들고 있으면 안 된다.</summary>
    public class BearerTokenHandler : DelegatingHandler
    {
        private readonly Func<string> accessTokenProvider;

        public BearerTokenHandler(HttpMessageHandler innerHandler, Func<string> accessTokenProvider) : base(innerHandler)
        {
            this.accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
        }

        public override UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string accessToken = accessTokenProvider.Invoke();

            if (string.IsNullOrEmpty(accessToken) == false)
            {
                request.Headers["Authorization"] = $"Bearer {accessToken}";
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
