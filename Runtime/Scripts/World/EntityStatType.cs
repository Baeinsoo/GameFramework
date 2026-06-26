namespace GameFramework.World
{
    /// <summary>
    /// 엔티티 primary 스탯 종류. <see cref="Stats"/>의 BaseStats/Modifiers dict 키로 (int) 캐스트해 사용한다.
    /// 새 스탯이 필요하면 값을 추가한다.
    /// </summary>
    public enum EntityStatType
    {
        Strength,
        Dexterity,
        Intelligence,
        Vitality,
        MoveSpeed,
    }
}
