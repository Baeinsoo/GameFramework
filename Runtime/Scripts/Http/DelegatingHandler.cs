using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    /// <summary>다음 핸들러를 감싸는 베이스. 상속해서 SendAsync를 재정의하고, 안쪽으로
    /// 넘길 때 base.SendAsync를 부른다.</summary>
    public class DelegatingHandler : HttpMessageHandler
    {
        protected HttpMessageHandler InnerHandler { get; }

        public DelegatingHandler(HttpMessageHandler innerHandler)
        {
            InnerHandler = innerHandler ?? throw new ArgumentNullException(nameof(innerHandler));
        }

        public override UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return InnerHandler.SendAsync(request, cancellationToken);
        }
    }
}
