using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework
{
    public class WebRequestParam<T> : IWebRequestParam<T>
    {
        public HttpMethod method { get; set; }
        public string uri { get; set; }
        public Dictionary<string, string> requestHeaders { get; set; }
        public object requestBody { get; set; }
        public List<IMultipartFormSection> form { get; set; }
        public Func<string, T> deserialize { get; set; }
        public IWebRequestInterceptor webRequestInterceptor { get; set; }
    }
}
