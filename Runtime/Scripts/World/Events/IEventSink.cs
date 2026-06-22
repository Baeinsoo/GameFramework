using System.Collections.Generic;

namespace GameFramework.World
{
    /// <summary>
    /// 확정된 WorldEvent를 바깥(프레젠테이션·네트워크)으로 송출하는 egress 포트.
    /// 순수 송출 — 상태를 바꾸거나 새 이벤트를 만들지 않는다(그건 Generation 소관).
    /// 사이드별 구현(WorldEventSink): 클라=프레젠테이션 EventBus, 서버=wire broadcast.
    /// </summary>
    public interface IEventSink
    {
        void Emit(IReadOnlyList<WorldEvent> events);
    }
}
