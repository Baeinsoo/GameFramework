using GameFramework.Auth;
using NUnit.Framework;
using UnityEngine;

namespace GameFramework.Tests.Auth
{
    public class PlayerPrefsAuthCredentialStoreTests
    {
        private const string Prefix = "Test.Auth";

        //  스토어가 내부적으로 만드는 키와 같은 형식 — 사람이 손으로 값을 망가뜨린 상황을
        //  재현하려면 스토어를 거치지 않고 PlayerPrefs에 직접 써야 하므로 여기 한 번 더 적는다.
        private const string DefaultProfileKey = Prefix + ".default.Credential";

        [TearDown]
        public void TearDown()
        {
            //  PlayerPrefs는 에디터 전역에 남는다 — 테스트가 서로의 값을 보지 않도록 지운다.
            new PlayerPrefsAuthCredentialStore(Prefix, "default").Clear();
            new PlayerPrefsAuthCredentialStore(Prefix, "Player2").Clear();
            PlayerPrefs.Save();
        }

        [Test]
        public void 저장한_것을_그대로_돌려준다()
        {
            var store = new PlayerPrefsAuthCredentialStore(Prefix, "default");
            store.Save(new AuthCredential { Provider = "ANONYMOUS", ProviderUserId = "p-1", Secret = "s-1" });

            var loaded = store.Load();

            Assert.AreEqual("ANONYMOUS", loaded.Provider);
            Assert.AreEqual("p-1", loaded.ProviderUserId);
            Assert.AreEqual("s-1", loaded.Secret);
        }

        [Test]
        public void 저장한_적_없으면_null이다()
        {
            Assert.IsNull(new PlayerPrefsAuthCredentialStore(Prefix, "default").Load());
        }

        [Test]
        public void 지우면_null이_된다()
        {
            var store = new PlayerPrefsAuthCredentialStore(Prefix, "default");
            store.Save(new AuthCredential { Provider = "ANONYMOUS", ProviderUserId = "p-1", Secret = "s-1" });

            store.Clear();

            Assert.IsNull(store.Load());
        }

        //  프로필 분리가 실제로 되는지 — 이게 깨지면 한 PC의 두 인스턴스가 같은 계정으로 붙는다.
        [Test]
        public void 프로필이_다르면_서로_보이지_않는다()
        {
            var first = new PlayerPrefsAuthCredentialStore(Prefix, "default");
            var second = new PlayerPrefsAuthCredentialStore(Prefix, "Player2");
            first.Save(new AuthCredential { Provider = "ANONYMOUS", ProviderUserId = "p-1", Secret = "s-1" });

            Assert.IsNull(second.Load());
            Assert.AreEqual("p-1", first.Load().ProviderUserId);
        }

        //  기기를 손으로 만져 저장된 문자열이 JSON조차 아니게 된 경우 — JsonUtility가 파싱
        //  단계에서 예외를 던지는데, 이걸 그대로 흘리면 로그인 화면 대신 앱이 죽는다.
        [Test]
        public void 파싱조차_안되는_문자열이면_null이다()
        {
            PlayerPrefs.SetString(DefaultProfileKey, "이건 JSON이 아니다 {{{");

            Assert.IsNull(new PlayerPrefsAuthCredentialStore(Prefix, "default").Load());
        }

        //  JSON 형태는 맞지만 필드가 비어 있는 경우 — JsonUtility는 예외 없이 필드가 null/빈
        //  문자열인 반쪽 객체를 돌려준다. 그걸 그대로 쓰면 반쪽 자격증명이 서버로 나간다.
        [Test]
        public void 필드가_비어있는_JSON이면_null이다()
        {
            PlayerPrefs.SetString(DefaultProfileKey, "{}");

            Assert.IsNull(new PlayerPrefsAuthCredentialStore(Prefix, "default").Load());
        }
    }
}
