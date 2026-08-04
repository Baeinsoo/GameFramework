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
            //  실제 위조 방지는 아래 서명 검증이 한다 — HMAC-SHA256을 헤더 내용과 무관하게 항상 돌리므로
            //  헤더가 alg를 none이나 다른 값으로 적어도 서명 없이는 통과할 수 없다.
            //  이 체크는 그 위에 얹는 defense-in-depth로, 형식이 어긋나거나 예상 밖인 헤더를 조기에 걸러낸다.
            if (header == null || header.Replace(" ", string.Empty).Contains(ExpectedHeaderAlgorithm) == false)
            {
                return false;
            }

            string computedSignature = ComputeSignature($"{parts[0]}.{parts[1]}", secret);
            if (ConstantTimeEquals(computedSignature, parts[2]) == false)
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

        //  일반 문자열 비교(!=)는 처음 다른 글자가 나오는 위치에서 바로 멈춘다 — 그 응답 시간 차이로
        //  서명을 한 글자씩 알아낼 수 있다(timing attack). 길이만 먼저 보고, 내용은 끝까지 다 훑어서
        //  비교한다(CryptographicOperations.FixedTimeEquals) — 어디까지 맞았는지 시간으로 새지 않는다.
        private static bool ConstantTimeEquals(string a, string b)
        {
            if (a.Length != b.Length)
            {
                return false;
            }

            byte[] aBytes = Encoding.UTF8.GetBytes(a);
            byte[] bBytes = Encoding.UTF8.GetBytes(b);
            return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
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

            string candidate = json.Substring(start, end - start);
            //  이스케이프(예: \")를 해석하지 않는다 — 값 안에 백슬래시가 있으면 진짜 닫는 따옴표를
            //  못 찾고 엉뚱한 위치에서 잘라 다른 사람의 subject를 돌려줄 위험이 있다. 값을 추측하지
            //  않고 실패로 처리한다(신원을 잘못 인정하는 것보다 거부가 안전).
            if (candidate.IndexOf('\\') >= 0)
            {
                return false;
            }

            value = candidate;
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
