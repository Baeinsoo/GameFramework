using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework
{
    public interface IWebRequestParam<T>
    {
        HttpMethod method { get; }
        string uri { get; }
        Dictionary<string, string> requestHeaders { get; }
        object requestBody { get; }
        List<IMultipartFormSection> form { get; }
        Func<string, T> deserialize { get; }
    }

    public enum HttpMethod
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
    }
}
