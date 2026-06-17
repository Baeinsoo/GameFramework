using System.Numerics;

namespace GameFramework.World
{
    /// <summary>엔티티의 공간 포즈(위치+회전). 순수 데이터(Anemic) — 로직은 System에 둔다.</summary>
    public class Transform : Component
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
    }
}
