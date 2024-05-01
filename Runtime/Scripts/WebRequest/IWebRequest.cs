using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework
{
    public interface IWebRequest<T> : IDisposable
    {
        T response { get; }

        UnityWebRequest unityWebRequest { get; }
        UnityWebRequestAsyncOperation asyncOperation { get; }
        IWebRequestParam<T> webRequestParam { get; }

        UnityWebRequest CreateUnityWebRequest();
    }
}
