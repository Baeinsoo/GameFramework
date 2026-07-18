using UnityEngine;

namespace GameFramework.World
{
    /// <summary>
    /// World.Entity의 모션(Transform/Velocity)을 UnityEngine.Vector3로 읽고 쓰는 경계 접근자.
    /// 코어는 System.Numerics(엔진 비의존)라 유니티↔순수숫자 변환이 필요한데, 그 변환과
    /// 불필요-쓰기 스킵(값이 같으면 안 씀)을 여기 한 곳에 모은다.
    /// </summary>
    public static class EntityMotionExtensions
    {
        public static Vector3 GetPosition(this Entity e) => e.Get<Transform>().Position.ToUnity();

        public static void SetPosition(this Entity e, Vector3 value)
        {
            var t = e.Get<Transform>();
            if (Vector3EqualityComparer.instance.Equals(t.Position.ToUnity(), value)) return;
            t.Position = value.ToNumerics();
        }

        public static Vector3 GetRotation(this Entity e) => e.Get<Transform>().Rotation.ToUnity().eulerAngles;

        public static void SetRotation(this Entity e, Vector3 eulerValue)
        {
            var t = e.Get<Transform>();
            if (Vector3EqualityComparer.instance.Equals(t.Rotation.ToUnity().eulerAngles, eulerValue)) return;
            t.Rotation = Quaternion.Euler(eulerValue).ToNumerics();
        }

        public static Vector3 GetVelocity(this Entity e) => e.Get<Velocity>().Linear.ToUnity();

        public static void SetVelocity(this Entity e, Vector3 value)
        {
            var v = e.Get<Velocity>();
            if (Vector3EqualityComparer.instance.Equals(v.Linear.ToUnity(), value)) return;
            v.Linear = value.ToNumerics();
        }
    }
}
