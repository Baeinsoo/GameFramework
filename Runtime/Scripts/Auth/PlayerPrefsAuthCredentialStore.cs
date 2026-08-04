using System;
using UnityEngine;

namespace GameFramework.Auth
{
    /// <summary>PlayerPrefs 기반 보관소.
    /// 주의: PlayerPrefs는 암호화되지 않는다. 기기를 만질 수 있는 사람은 값을 꺼낼 수 있다.</summary>
    public class PlayerPrefsAuthCredentialStore : IAuthCredentialStore
    {
        private readonly string key;

        public PlayerPrefsAuthCredentialStore(string keyPrefix, string profile)
        {
            //  프로필을 키에 섞어야 한 기기에서 인스턴스마다 다른 계정을 쓸 수 있다.
            key = $"{keyPrefix}.{profile}.Credential";
        }

        public AuthCredential Load()
        {
            string json = PlayerPrefs.GetString(key, string.Empty);
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }

            AuthCredential credential;
            try
            {
                credential = JsonUtility.FromJson<AuthCredential>(json);
            }
            catch (ArgumentException ex)
            {
                //  기기를 손으로 만져 저장값이 JSON 자체가 아니게 된 경우 — JsonUtility가 파싱
                //  단계에서 예외를 던진다. 앱을 죽이는 대신 "자격증명 없음"으로 취급해
                //  로그인 화면으로 보낸다.
                Debug.LogWarning($"[Auth] 저장된 자격증명 파싱에 실패해 자격증명 없음으로 취급합니다: {ex.Message}");
                return null;
            }

            //  JsonUtility는 문법은 맞지만 필드가 비거나 다른 JSON(예: "{}")이 와도 예외 없이
            //  null/빈 문자열이 채워진 반쪽 객체를 돌려준다. 그런 반쪽 자격증명을 서버로 그대로
            //  보내는 것은 자격증명이 아예 없는 것보다 위험하므로, 필드 하나라도 비어 있으면
            //  없는 것으로 취급한다.
            if (credential == null
                || string.IsNullOrEmpty(credential.Provider)
                || string.IsNullOrEmpty(credential.ProviderUserId)
                || string.IsNullOrEmpty(credential.Secret))
            {
                return null;
            }

            return credential;
        }

        public void Save(AuthCredential credential)
        {
            PlayerPrefs.SetString(key, JsonUtility.ToJson(credential));
            PlayerPrefs.Save();
        }

        public void Clear()
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }
    }
}
