using System;

namespace GameFramework.World
{
    /// <summary>캡슐이 눕는 방향. Y는 기존 선 캡슐, X는 누운 캡슐(가로로 긴 몸).</summary>
    public enum CapsuleAxis
    {
        Y,
        X,
    }

    /// <summary>
    /// 엔티티 몸의 캡슐 치수. 맵에 부딪히는지 보는 sweep과, 물리 엔진에 세우는 콜라이더가
    /// 이 하나를 같이 본다 — 두 곳이 다른 값을 들고 있으면 시뮬이 모르는 위치 보정이 매 틱 끼어든다.
    /// 값은 게임이 정한다(엔티티를 만드는 쪽이 붙인다).
    /// </summary>
    public class CapsuleShape : Component
    {
        public float Radius { get; }

        /// <summary>발밑부터 정수리까지 전체 높이(세로). 누운 캡슐이면 지름과 같다.</summary>
        public float Height { get; }

        /// <summary>축 방향 전체 길이. Y 캡슐이면 Height와 같다.</summary>
        public float Length { get; }

        /// <summary>캡슐이 누운 축.</summary>
        public CapsuleAxis Axis { get; }

        public CapsuleShape(float radius, float height) : this(radius, height, height, CapsuleAxis.Y) { }

        /// <summary>X축으로 누운 캡슐. 세로(Height)는 지름, 가로(Length)는 length.</summary>
        public static CapsuleShape Lying(float radius, float length) =>
            new CapsuleShape(radius, radius * 2f, length, CapsuleAxis.X);

        private CapsuleShape(float radius, float height, float length, CapsuleAxis axis)
        {
            if (radius <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), radius, "몸 반지름은 0보다 커야 한다.");
            }
            if (height <= 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "몸 높이는 0보다 커야 한다.");
            }
            if (height < radius * 2f)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height,
                    "몸 높이는 지름(반지름×2)보다 작을 수 없다 — 그보다 낮으면 sweep과 물리 콜라이더가 서로 다른 몸을 본다.");
            }
            if (length < radius * 2f)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length,
                    "몸 길이는 지름(반지름×2)보다 작을 수 없다 — 그보다 짧으면 sweep과 물리 콜라이더가 서로 다른 몸을 본다.");
            }

            Radius = radius;
            Height = height;
            Length = length;
            Axis = axis;
        }
    }
}
