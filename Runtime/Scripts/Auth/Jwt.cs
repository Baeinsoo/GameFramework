using System;
using System.Security.Cryptography;
using System.Text;

namespace GameFramework.Auth
{
    /// <summary>서버가 발급한 HS256 JWT를 검증한다. 발급은 서버 몫이고 여기서는 검증만 한다.</summary>
    public static class Jwt
    {
        private const string ExpectedHeaderAlgorithm = "\"alg\":\"HS256\"";

        public static bool TryVerifyHs256(string token, string secret, out string subject, DateTimeOffset now)
        {
            subject = null;

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(secret))
            {
                return false;
            }

            string[] parts = token.Split('.');
            if (parts.Length != 3)
            {
                return false;
            }

            string header = DecodeToString(parts[0]);
            //  알고리즘을 고정하지 않으면 alg를 none이나 비대칭으로 바꾼 토큰이 통과한다.
            if (header == null || header.Replace(" ", string.Empty).Contains(ExpectedHeaderAlgorithm) == false)
            {
                return false;
            }

            if (ComputeSignature($"{parts[0]}.{parts[1]}", secret) != parts[2])
            {
                return false;
            }

            string payload = DecodeToString(parts[1]);
            if (payload == null)
            {
                return false;
            }

            if (TryReadNumber(payload, "exp", out long exp) == false || exp <= now.ToUnixTimeSeconds())
            {
                return false;
            }

            if (TryReadString(payload, "sub", out string sub) == false || string.IsNullOrEmpty(sub))
            {
                return false;
            }

            subject = sub;
            return true;
        }

        private static string ComputeSignature(string signingInput, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            return Base64Url(hmac.ComputeHash(Encoding.UTF8.GetBytes(signingInput)));
        }

        private static string Base64Url(byte[] bytes)
        {
            return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
        }

        private static string DecodeToString(string base64Url)
        {
            try
            {
                string padded = base64Url.Replace('-', '+').Replace('_', '/');
                padded = padded.PadRight(padded.Length + (4 - padded.Length % 4) % 4, '=');
                return Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            }
            catch
            {
                return null;
            }
        }

        //  JSON 파서를 끌어오지 않는 이유: 이 payload는 우리 서버가 만든 두 필드짜리 고정 형태다.
        private static bool TryReadString(string json, string key, out string value)
        {
            value = null;
            int start = json.IndexOf($"\"{key}\":\"", StringComparison.Ordinal);
            if (start < 0)
            {
                return false;
            }

            start += key.Length + 4;
            int end = json.IndexOf('"', start);
            if (end < 0)
            {
                return false;
            }

            value = json.Substring(start, end - start);
            return true;
        }

        private static bool TryReadNumber(string json, string key, out long value)
        {
            value = 0;
            int start = json.IndexOf($"\"{key}\":", StringComparison.Ordinal);
            if (start < 0)
            {
                return false;
            }

            start += key.Length + 3;
            int end = start;
            while (end < json.Length && char.IsDigit(json[end]))
            {
                end++;
            }

            return end > start && long.TryParse(json.Substring(start, end - start), out value);
        }
    }
}
