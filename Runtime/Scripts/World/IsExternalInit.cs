// C# 9 record 지원을 위한 폴리필. Unity 6의 .NET Standard 2.1 BCL은 IsExternalInit를 제공하지 않는다
// (이 타입은 .NET 5+에서 추가됨). noEngineReferences:true 어셈블리에서 record 컴파일을 가능하게 한다.
// 이 어셈블리(baegames.GameFramework.World)에서만 internal 노출, 다른 어셈블리에 영향 없음.
namespace System.Runtime.CompilerServices
{
    internal static class IsExternalInit
    {
    }
}
