using System.Numerics;

namespace GameFramework.World
{
    /// <summary>엔티티의 운동(선속도). 각속도는 현재 미사용 — 필요 시 Angular 추가. 순수 데이터.</summary>
    public class Velocity : Component
    {
        public Vector3 Linear { get; set; }
    }
}
