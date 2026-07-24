namespace GameFramework
{
    /// <summary>
    /// 틱 파이프라인의 한 스텝. Runner가 페이즈별로 등록된 순서대로 Tick을 호출한다.
    /// (구 리플렉션 이벤트버스를 대체 — 타입드 등록, 런타임 add/remove 지원.)
    /// </summary>
    public interface ITickSystem
    {
        void Tick(long tick, float deltaTime);
    }
}
