using System;
using UnityEngine;

namespace GameFramework
{
    /// <summary>
    /// 이 어트리뷰트가 붙은 컴포넌트가 있는 GameObject는 자신이 속한 씬이 로드될 때
    /// SceneLifetimeScope의 스캔에 의해 InjectGameObject로 주입된다 —
    /// 해당 GameObject와 그 자식 컴포넌트들에서 [Inject]가 붙은 멤버(필드/프로퍼티/메서드)가 모두 채워진다.
    /// MonoBehaviour는 Unity가 생성하므로 생성자 주입은 불가능하고, 그래서 [Inject] 멤버로만 주입된다.
    /// (최초 씬뿐 아니라 additive로 로드되는 씬도 포함.)
    ///
    /// 동작/한계:
    /// - 주입을 "받기"만 한다. 컨테이너에 등록되지는 않으므로 다른 타입이 이 객체를
    ///   [Inject]/Resolve로 받을 수는 없다. (남이 받아야 하면 RegisterComponent를 쓸 것.)
    /// - 스캔은 씬 로드 타이밍에만 실행된다. 로드 이후 런타임에 Instantiate/AddComponent로
    ///   생성된 객체는 스캔 대상이 아니므로 IObjectResolver.Instantiate / Inject로 직접 주입해야 한다.
    /// - 단일 컴포넌트만 주입하려면 [DIMonoBehaviour]를 사용한다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DIGameObjectAttribute : Attribute { }
}
