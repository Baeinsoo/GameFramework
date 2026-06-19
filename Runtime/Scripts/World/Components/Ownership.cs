namespace GameFramework.World
{
    /// <summary>
    /// 엔티티가 유저/세션에 의해 소유·제어됨을 표시한다. 존재 자체가 "플레이어(비-NPC)" 마커.
    /// <see cref="OwnerId"/>는 소유자 식별자(LOP=userId), 생성 시 1회 세팅 후 불변.
    /// </summary>
    public class Ownership : Component
    {
        public string OwnerId { get; }

        public Ownership(string ownerId)
        {
            OwnerId = ownerId;
        }
    }
}
