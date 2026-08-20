using System.Numerics;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 한 틱의 엔티티 시뮬 상태 사진(위치·회전·속도). 엔진 비의존(System.Numerics) 순수 데이터.
    /// 클라 롤백 예측과 서버 lag-compensation이 공유한다.
    /// </summary>
    public readonly struct EntitySnapshot
    {
        public long Tick { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }
        public Vector3 Velocity { get; }

        public EntitySnapshot(long tick, Vector3 position, Quaternion rotation, Vector3 velocity)
        {
            Tick = tick;
            Position = position;
            Rotation = rotation;
            Velocity = velocity;
        }
    }
}
