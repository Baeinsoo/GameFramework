using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    /// <summary>요청마다 지정한 헤더에 API 키를 붙인다. 서비스끼리 부르는 호출에 쓴다.</summary>
    public class ApiKeyHandler : DelegatingHandler
    {
        private readonly string headerName;
        private readonly Func<string> keyProvider;

        public ApiKeyHandler(HttpMessageHandler innerHandler, string headerName, Func<string> keyProvider) : base(innerHandler)
        {
            if (string.IsNullOrEmpty(headerName))
            {
                throw new ArgumentException("헤더 이름이 필요하다.", nameof(headerName));
            }

            this.headerName = headerName;
            this.keyProvider = keyProvider ?? throw new ArgumentNullException(nameof(keyProvider));
        }

        public override UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //  보낼 때마다 다시 읽는다 — 환경변수가 프로세스 시작 뒤에 채워질 수 있어,
            //  만들 때 한 번 읽으면 빈 값이 그대로 굳는다.
            string key = keyProvider();

            //  키가 없으면 헤더를 붙이지 않는다. 빈 값을 보내면 서버가 "틀린 키"로 읽어
            //  401을 주고, 설정이 빠진 것인지 키가 틀린 것인지 구분하기 어려워진다.
            if (string.IsNullOrEmpty(key) == false)
            {
                request.Headers[headerName] = key;
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
