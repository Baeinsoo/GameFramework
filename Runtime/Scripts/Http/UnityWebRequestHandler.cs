using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace GameFramework.Http
{
    /// <summary>체인의 끝. 실제로 네트워크에 나가는 유일한 곳이다.</summary>
    public class UnityWebRequestHandler : HttpMessageHandler
    {
        public override async UniTask<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            using (UnityWebRequest unityWebRequest = Create(request))
            {
                try
                {
                    await unityWebRequest.SendWebRequest().WithCancellation(cancellationToken);
                }
                catch (UnityWebRequestException)
                {
                    //  UniTask는 4xx·5xx에도 예외를 던진다. 우리 계약은 "상태코드는 응답으로
                    //  돌려준다"이므로(핸들러가 401을 보고 재시도할 수 있어야 한다) 여기서 삼키고
                    //  아래에서 result로 다시 판정한다. 취소는 OperationCanceledException이라
                    //  이 catch에 안 걸리고 그대로 올라간다.
                }

                if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                    unityWebRequest.result == UnityWebRequest.Result.DataProcessingError)
                {
                    //  서버에 닿지 못했거나 응답을 읽지 못했다 — 상태코드가 없다는 것이 그 신호다.
                    throw new HttpRequestException(
                        $"요청 전송에 실패했습니다. uri: {request.Uri}, error: {unityWebRequest.error}");
                }

                return new HttpResponseMessage(
                    unityWebRequest.responseCode,
                    unityWebRequest.downloadHandler?.text ?? string.Empty,
                    unityWebRequest.GetResponseHeaders());
            }
        }

        private static UnityWebRequest Create(HttpRequestMessage request)
        {
            var unityWebRequest = new UnityWebRequest(request.Uri, request.Method.ToString());
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            if (string.IsNullOrEmpty(request.Content) == false)
            {
                unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(request.Content));
                unityWebRequest.SetRequestHeader("Content-Type", request.ContentType);
            }

            //  GameFramework.HttpExtensions 의 확장 — 바깥 네임스페이스라 using 없이 잡힌다.
            unityWebRequest.SetRequestHeader(request.Headers);

            return unityWebRequest;
        }
    }
}
