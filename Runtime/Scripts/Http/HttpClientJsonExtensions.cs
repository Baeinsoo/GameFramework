using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    /// <summary>응답을 T로 바꿔 주는 계층. 여기서만 상태코드를 예외로 바꾼다 —
    /// 핸들러 체인은 4xx·5xx도 응답으로 넘겨야 재시도 판단이 가능하다.</summary>
    public static class HttpClientJsonExtensions
    {
        public static UniTask<T> SendAsync<T>(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
        {
            return client.SendAsync(request, HttpJson.DeserializeObject<T>, cancellationToken);
        }

        public static async UniTask<T> SendAsync<T>(this HttpClient client, HttpRequestMessage request, Func<string, T> deserialize, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await client.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            return deserialize(response.Body);
        }
    }
}
