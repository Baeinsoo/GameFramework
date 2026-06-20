using UnityEngine;

namespace GameFramework
{
    public static partial class Extensions
    {
        /// <summary>
        /// UnityEngine → System.Numerics 변환. World 코어(noEngineReferences)는 System.Numerics만,
        /// 엔진 측(클라/서버)은 UnityEngine만 보므로 이 변환이 둘의 경계를 잇는다.
        /// (World→Unity 역변환은 pull 소비가 생기는 후속 슬라이스에서 추가.)
        /// </summary>
        public static System.Numerics.Vector3 ToNumerics(this Vector3 v)
            => new System.Numerics.Vector3(v.x, v.y, v.z);

        public static System.Numerics.Quaternion ToNumerics(this Quaternion q)
            => new System.Numerics.Quaternion(q.x, q.y, q.z, q.w);
    }
}
