using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;

namespace GameFramework
{
    public class WebRequestBuilder<T>
    {
        private WebRequestParam<T> webRequestParam = new WebRequestParam<T>();

        public WebRequestBuilder<T> SetMethod(HttpMethod method)
        {
            webRequestParam.method = method;
            return this;
        }

        public WebRequestBuilder<T> SetUri(string uri)
        {
            webRequestParam.uri = uri;
            return this;
        }

        public WebRequestBuilder<T> SetRequestHeader(Dictionary<string, string> requestHeaders)
        {
            webRequestParam.requestHeaders = requestHeaders;
            return this;
        }

        public WebRequestBuilder<T> SetRequestBody(object requestBody)
        {
            webRequestParam.requestBody = requestBody;
            return this;
        }

        public WebRequestBuilder<T> SetForm(List<IMultipartFormSection> form)
        {
            webRequestParam.form = form;
            return this;
        }

        public WebRequestBuilder<T> SetDeserialize(Func<string, T> deserialize)
        {
            webRequestParam.deserialize = deserialize;
            return this;
        }

        public WebRequest<T> Build()
        {
            return new WebRequest<T>(webRequestParam);
        }
    }
}
