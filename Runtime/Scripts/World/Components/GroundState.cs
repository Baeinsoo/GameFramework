namespace GameFramework.World
{
    /// <summary>
    /// 캐릭터의 지면 접촉 상태. 키네마틱 이동이 매 틱 갱신하고, 뷰(애니)와 네트워크 스냅샷이 읽는다.
    /// 지금은 지상/공중 두 상태뿐이라 bool — 수영·비행이 생기면 MovementMode enum이 될 자리.
    /// </summary>
    public class GroundState : Component
    {
        public bool IsGrounded { get; set; }
    }
}
