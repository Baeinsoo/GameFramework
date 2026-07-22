using System;

namespace GameFramework
{
    /// <summary>
    /// 씬에 미리 배치된(정적) GameObject를 표시한다. 씬 로드 시 씬 스코프의 스캔이
    /// 이 GameObject와 그 하위 모든 컴포넌트에 VContainer 주입을 수행한다
    /// (InjectGameObject). 런타임 생성 오브젝트는 대상이 아니며 별도 주입이 필요하다.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SceneInjectGameObjectAttribute : Attribute { }
}
