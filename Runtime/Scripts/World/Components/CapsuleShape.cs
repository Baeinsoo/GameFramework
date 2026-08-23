using System;

namespace GameFramework.World
{
    /// <summary>
    /// 엔티티 몸의 캡슐 치수. 맵에 부딪히는지 보는 sweep과, 물리 엔진에 세우는 콜라이더가
    /// 이 하나를 같이 본다 — 두 곳이 다른 값을 들고 있으면 시뮬이 모르는 위치 보정이 매 틱 끼어든다.
    /// 값은 게임이 정한다(엔티티를 만드는 쪽이 붙인다).
    /// </summary>
    public class CapsuleShape : Component
    {
        public float Radius { get; }

        /// <summary>발밑부터 정수리까지 전체 높이.</summary>
        public float Height { get; }

        public CapsuleShape(float radius, float height)
        {
            if (radius <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "몸 반지름은 0보다 커야 한다.");
            }
            if (height <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "몸 높이는 0보다 커야 한다.");
            }

            Radius = radius;
            Height = height;
        }
    }
}
