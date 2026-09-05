using System.Numerics;

namespace GameFramework.World
{
    /// <summary>엔티티의 공간 포즈(위치+회전). 순수 데이터(Anemic) — 로직은 System에 둔다.</summary>
    public class Transform : Component
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.Identity;

        /// <summary>
        /// 순간이동한 횟수. 받는 쪽은 <b>값이 바뀌었는지</b>만 본다 — 늘 켜져 있는 표시가 아니라
        /// "직전에 본 것과 다른가"라서, 스냅샷 하나가 유실돼도 다음 스냅샷에서 알아챈다.
        /// </summary>
        public int TeleportCount { get; set; }
    }
}
