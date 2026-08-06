using System.Collections.Generic;

namespace GameFramework.Http
{
    /// <summary>한 번의 HTTP 요청을 담는 데이터. 핸들러가 헤더를 덧붙일 수 있고,
    /// 같은 인스턴스를 다시 보내도 된다(재시도 핸들러가 그렇게 쓴다).</summary>
    public class HttpRequestMessage
    {
        public HttpMethod Method { get; }
        public string Uri { get; }
        public Dictionary<string, string> Headers { get; } = new Dictionary<string, string>();
        public string Content { get; }
        public string ContentType { get; }

        public HttpRequestMessage(HttpMethod method, string uri, string content = null, string contentType = null)
        {
            Method = method;
            Uri = uri;
            Content = content;
            ContentType = contentType;
        }

        public static HttpRequestMessage Get(string uri) => new HttpRequestMessage(HttpMethod.GET, uri);

        public static HttpRequestMessage Delete(string uri) => new HttpRequestMessage(HttpMethod.DELETE, uri);

        public static HttpRequestMessage Post(string uri, object body = null) => Json(HttpMethod.POST, uri, body);

        public static HttpRequestMessage Put(string uri, object body = null) => Json(HttpMethod.PUT, uri, body);

        private static HttpRequestMessage Json(HttpMethod method, string uri, object body)
        {
            return body == null
                ? new HttpRequestMessage(method, uri)
                : new HttpRequestMessage(method, uri, HttpJson.SerializeObject(body), "application/json");
        }
    }
}
