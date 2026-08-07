using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    /// <summary>요청에 실을 토큰을 준다. 필요하면 갱신까지 하고 준다 — 부르는 쪽은 갱신을 모른다.</summary>
    public interface IAccessTokenProvider
    {
        /// <param name="forceRefresh">만료가 남았어도 새로 받아온다. 401을 맞은 뒤에 쓴다.</param>
        /// <returns>실을 토큰. 로그인 상태가 아니면 null. 갱신에 실패하면 지금 가진 토큰을 그대로 준다
        /// — 갱신 실패는 "이 토큰이 죽었다"는 뜻이 아니라 서버에 못 물어봤다는 뜻이다.</returns>
        UniTask<string> GetAccessTokenAsync(bool forceRefresh, CancellationToken cancellationToken);
    }
}
