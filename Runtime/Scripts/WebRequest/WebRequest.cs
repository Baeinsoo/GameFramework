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
        public override bool keepWaiting
        {
            get
            {
                bool isDone = asyncOperation != null && asyncOperation.isDone;
                return isDone == false;
            }
        }

        public UnityWebRequestAsyncOperation asyncOperation { get; private set; }
        public event Action completed;
        public bool isSuccess => unityWebRequest.result == UnityWebRequest.Result.Success;

        public UnityWebRequest unityWebRequest { get; private set; }
        public IWebRequestParam<T> webRequestParam { get; private set; }

        public T response { get; private set; }

        public WebRequest(IWebRequestParam<T> webRequestParam)
        {
            if (webRequestParam == null)
            {
                throw new Exception("webRequestParam must not null.");
            }

            this.webRequestParam = webRequestParam;
            this.unityWebRequest = CreateUnityWebRequest();
            this.asyncOperation = this.unityWebRequest.SendWebRequest();
            this.asyncOperation.completed += _ =>
            {
                response = (webRequestParam.deserialize ?? JsonConvert.DeserializeObject<T>).Invoke(unityWebRequest.downloadHandler.text);

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
                var bodyJson = JsonConvert.SerializeObject(webRequestParam.requestBody, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });
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
