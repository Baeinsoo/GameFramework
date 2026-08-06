using System;

namespace GameFramework.Http
{
    public class HttpRequestException : Exception
    {
        /// <summary>서버가 답한 HTTP 상태. <c>null</c>이면 서버에 닿지도 못한 것(연결 실패·타임아웃).
        /// "서버가 거부했다(401)"와 "물어보지 못했다"를 반드시 구분해야 하므로 nullable이다.</summary>
        public long? StatusCode { get; }

        public string ResponseBody { get; }

        public HttpRequestException(string message) : base(message) { }

        public HttpRequestException(string message, long statusCode, string responseBody) : base(message)
        {
            StatusCode = statusCode;
            ResponseBody = responseBody;
        }
    }
}
