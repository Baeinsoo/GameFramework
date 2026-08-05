namespace GameFramework.Auth
{
    /// <summary>자격증명 보관소. 저장 위치를 바꿀 수 있도록 인터페이스로 둔다
    /// (지금은 PlayerPrefs, 계정에 지킬 가치가 생기면 플랫폼 보안 저장소로 교체).</summary>
    public interface IAuthCredentialStore
    {
        AuthCredential Load();
        void Save(AuthCredential credential);
        void Clear();
    }
}
