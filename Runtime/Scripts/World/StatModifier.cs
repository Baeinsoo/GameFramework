namespace GameFramework.World
{
    /// <summary>
    /// 스탯 보정값 하나(데이터). 출처(<see cref="SourceId"/>)별로 추적되어 일괄 해제 가능하다.
    /// 스탯 식별자(<see cref="StatType"/>)는 게임이 정의한 enum을 int로 캐스팅해 사용한다.
    /// </summary>
    public readonly struct StatModifier
    {
        public int StatType { get; }
        public float Value { get; }
        public ModifierType Type { get; }
        public string SourceId { get; }

        public StatModifier(int statType, float value, ModifierType type, string sourceId)
        {
            StatType = statType;
            Value = value;
            Type = type;
            SourceId = sourceId;
        }
    }
}
