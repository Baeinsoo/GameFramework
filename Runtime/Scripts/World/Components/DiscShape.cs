using System;

namespace GameFramework.World
{
    /// <summary>
    /// 원반 몸의 치수(동전처럼 얇고 둥근 것). <see cref="CapsuleShape"/>과 같은 자리 —
    /// 순수 기하만 담고 엔진 설정은 <see cref="PhysicsConfig"/>가 갖는다.
    /// 값은 게임이 정한다(엔티티를 만드는 쪽이 붙인다).
    /// </summary>
    public class DiscShape : Component
    {
        public float Radius { get; }

        /// <summary>원반의 두께(높이).</summary>
        public float Thickness { get; }

        public DiscShape(float radius, float thickness)
        {
            if (radius <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "원반 반지름은 0보다 커야 한다.");
            }
            if (thickness <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(thickness), thickness, "원반 두께는 0보다 커야 한다.");
            }

            Radius = radius;
            Thickness = thickness;
        }
    }
}
