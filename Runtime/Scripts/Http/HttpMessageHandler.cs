using System.Threading;
using Cysharp.Threading.Tasks;

namespace GameFramework.Http
{
    public abstract class HttpMessageHandler
    {
        public abstract UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}
