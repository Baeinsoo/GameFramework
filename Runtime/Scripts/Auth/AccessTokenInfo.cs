using System;

namespace GameFramework.Auth
{
    /// <summary>서버가 발급한 액세스 토큰과 그 만료 시각. 갱신 시점 판단만 담당한다.</summary>
    public readonly struct AccessTokenInfo
    {
        /// <summary>만료 몇 분 전부터 갱신할지. 요청이 만료된 토큰을 만나 401이 나기 전에 미리 바꾼다.</summary>
        public static readonly TimeSpan DefaultRefreshMargin = TimeSpan.FromMinutes(5);

        public string Token { get; }
        public DateTimeOffset ExpiresAt { get; }

        private AccessTokenInfo(string token, DateTimeOffset expiresAt)
        {
            Token = token;
            ExpiresAt = expiresAt;
        }

        public static AccessTokenInfo FromExpiresIn(string token, int expiresInSeconds, DateTimeOffset issuedAt)
        {
            return new AccessTokenInfo(token, issuedAt.AddSeconds(expiresInSeconds));
        }

        public bool IsExpired(DateTimeOffset now)
        {
            return now >= ExpiresAt;
        }

        public bool NeedsRefresh(DateTimeOffset now, TimeSpan margin)
        {
            return now >= ExpiresAt - margin;
        }
    }
}
