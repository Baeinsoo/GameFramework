using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using Newtonsoft.Json;
using System.Text;

namespace GameFramework
{
    public class WebRequest<T> : CustomYieldInstruction, IWebRequest<T>
    {
        public override bool keepWaiting => !isCompleted;

        public UnityWebRequestAsyncOperation asyncOperation { get; private set; }
        public event Action completed;
        public bool isSuccess => unityWebRequest.result == UnityWebRequest.Result.Success;
        public string error => unityWebRequest.error;
        public long responseCode => unityWebRequest.responseCode;

        public UnityWebRequest unityWebRequest { get; private set; }
        public IWebRequestParam<T> webRequestParam { get; private set; }

        public T response { get; private set; }

        private bool isCompleted = false;

        public WebRequest(IWebRequestParam<T> webRequestParam)
        {
            if (webRequestParam == null)
            {
                throw new Exception("webRequestParam must not null.");
            }

            this.webRequestParam = webRequestParam;
            this.unityWebRequest = CreateUnityWebRequest();
            this.webRequestParam.webRequestInterceptor?.OnBeforeRequest(this.unityWebRequest);
            this.asyncOperation = this.unityWebRequest.SendWebRequest();
            this.asyncOperation.completed += _ =>
            {
                if (isSuccess)
                {
                    response = (webRequestParam.deserialize ?? WebRequestJson.DeserializeObject<T>).Invoke(unityWebRequest.downloadHandler.text);

                    webRequestParam.webRequestInterceptor?.OnSuccess(this.unityWebRequest, response);
                }
                else
                {
                    Debug.LogWarning($"{webRequestParam.uri} is not success. unityWebRequest.result: {unityWebRequest.result}, error: {error}");

                    webRequestParam.webRequestInterceptor?.OnError(this.unityWebRequest, error);
                }

                isCompleted = true;

                completed?.Invoke();
            };
        }

        public void Dispose()
        {
            unityWebRequest?.Dispose();
        }

        public virtual UnityWebRequest CreateUnityWebRequest()
        {
            var unityWebRequest = webRequestParam.form == null ? CreateDefaultUnityWebRequest() : CreateFormUnityWebRequest();

            unityWebRequest.SetRequestHeader(webRequestParam.requestHeaders);
            unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

            return unityWebRequest;
        }

        private UnityWebRequest CreateDefaultUnityWebRequest()
        {
            var unityWebRequest = new UnityWebRequest(webRequestParam.uri, webRequestParam.method.ToString());

            if (webRequestParam.requestBody != null)
            {
                var bodyJson = WebRequestJson.SerializeObject(webRequestParam.requestBody);
                byte[] data = new UTF8Encoding().GetBytes(bodyJson);
                unityWebRequest.uploadHandler = new UploadHandlerRaw(data);
                unityWebRequest.SetRequestHeader("Content-Type", "application/json");
            }

            return unityWebRequest;
        }

        private UnityWebRequest CreateFormUnityWebRequest()
        {
            if (webRequestParam.method != HttpMethod.POST)
            {
                throw new Exception($"Only post method is valid to send Form. current method: {webRequestParam.method}");
            }

            return UnityWebRequest.Post(webRequestParam.uri, webRequestParam.form);
        }
    }
}
