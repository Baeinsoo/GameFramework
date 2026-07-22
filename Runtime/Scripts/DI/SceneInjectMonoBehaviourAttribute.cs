using System;

namespace GameFramework
{
    /// <summary>
    /// 씬에 미리 배치된(정적) MonoBehaviour를 표시한다. 씬 로드 시 씬 스코프의 스캔이
    /// 이 컴포넌트 하나에만 VContainer 주입을 수행한다(하위 자식 제외). 런타임에
    /// Instantiate/AddComponent로 생성되는 오브젝트는 이 스캔 대상이 아니며 별도로
    /// 주입해야 한다(IObjectResolver.Instantiate / Inject).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneInjectMonoBehaviourAttribute : Attribute { }
}
