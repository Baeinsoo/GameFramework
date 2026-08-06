using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    /// <summary>요청에 Authorization 헤더를 붙인다. 401이면 토큰을 갱신해 딱 한 번 다시 보낸다.</summary>
    public class BearerTokenHandler : DelegatingHandler
    {
        private const long HttpStatusUnauthorized = 401;

        private readonly IAccessTokenProvider accessTokenProvider;

        public BearerTokenHandler(HttpMessageHandler innerHandler, IAccessTokenProvider accessTokenProvider) : base(innerHandler)
        {
            this.accessTokenProvider = accessTokenProvider ?? throw new ArgumentNullException(nameof(accessTokenProvider));
        }

        public override async UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //  매번 물어보는 이유: 만료가 임박했으면 공급자가 이 안에서 갱신해 새 토큰을 준다.
            string accessToken = await accessTokenProvider.GetAccessTokenAsync(false, cancellationToken);
            SetAuthorization(request, accessToken);

            HttpResponseMessage response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode != HttpStatusUnauthorized)
            {
                return response;
            }

            string refreshed = await accessTokenProvider.GetAccessTokenAsync(true, cancellationToken);

            //  토큰이 그대로면 갱신이 실패한 것이다. 방금 거부당한 토큰을 다시 보내봐야 결과가 같으므로
            //  헛수고 대신 원래 401을 돌려준다.
            if (string.IsNullOrEmpty(refreshed) || refreshed == accessToken)
            {
                return response;
            }

            SetAuthorization(request, refreshed);

            //  재전송은 여기 한 번뿐이다 — 이 응답이 또 401이어도 그대로 반환되므로 루프가 될 수 없다.
            return await base.SendAsync(request, cancellationToken);
        }

        private static void SetAuthorization(HttpRequestMessage request, string accessToken)
        {
            //  토큰이 없으면 헤더를 붙이지 않는다 — 빈 Bearer를 보내면 서버가 잘못된 토큰으로 읽는다.
            if (string.IsNullOrEmpty(accessToken))
            {
                return;
            }

            request.Headers["Authorization"] = $"Bearer {accessToken}";
        }
    }
}
