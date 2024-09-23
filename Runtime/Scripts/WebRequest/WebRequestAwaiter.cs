using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace GameFramework
{
    public class WebRequestAwaiter<T> : INotifyCompletion
    {
        private WebRequest<T> webRequest;
        private Action continuation;

        public WebRequestAwaiter(WebRequest<T> webRequest)
        {
            this.webRequest = webRequest;
            this.webRequest.completed += OnRequestCompleted;
        }

        public bool IsCompleted => !webRequest.keepWaiting;

        public WebRequest<T> GetResult()
        {
            if (webRequest.isSuccess == false)
            {
                throw new WebRequestException($"Request failed with error: {webRequest.error}");
            }
         
            return webRequest;
        }
        
        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        private void OnRequestCompleted()
        {
            continuation?.Invoke();
        }
    }
}
