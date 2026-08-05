using System;

namespace GameFramework.Auth
{
    /// <summary>다음 실행에서 다시 로그인하기 위해 기기에 보관하는 자격증명.</summary>
    [Serializable]
    public class AuthCredential
    {
        public string Provider;
        public string ProviderUserId;
        public string Secret;
    }
}
