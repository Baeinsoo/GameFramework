using UnityEngine.Networking;

namespace GameFramework
{
    public interface IWebRequestInterceptor
    {
        void OnBeforeRequest(UnityWebRequest request);
        void OnSuccess<T>(UnityWebRequest request, T response);
        void OnError(UnityWebRequest request, string error);
    }
}
