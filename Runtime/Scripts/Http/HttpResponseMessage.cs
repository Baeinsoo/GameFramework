using System.Collections.Generic;

namespace GameFramework.Http
{
    /// <summary>서버가 실제로 답한 내용. 4xx·5xx도 정상적인 "응답"이라 여기까지 온다 —
    /// 예외로 바꿀지는 호출하는 쪽이 EnsureSuccessStatusCode로 정한다.</summary>
    public class HttpResponseMessage
    {
        private static readonly IReadOnlyDictionary<string, string> EmptyHeaders = new Dictionary<string, string>();

        public long StatusCode { get; }
        public string Body { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }

        public HttpResponseMessage(long statusCode, string body, IReadOnlyDictionary<string, string> headers = null)
        {
            StatusCode = statusCode;
            Body = body;
            Headers = headers ?? EmptyHeaders;
        }

        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode <= 299;

        public HttpResponseMessage EnsureSuccessStatusCode()
        {
            if (IsSuccessStatusCode == false)
            {
                throw new HttpRequestException($"HTTP {StatusCode} 응답을 받았습니다.", StatusCode, Body);
            }

            return this;
        }
    }
}
