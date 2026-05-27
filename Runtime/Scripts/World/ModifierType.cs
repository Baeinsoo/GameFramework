namespace GameFramework.World
{
    /// <summary>스탯 모디파이어의 합성 방식. 적용 순서: Flat → PercentAdd → PercentMult.</summary>
    public enum ModifierType
    {
        /// <summary>기본값에 가산 (+10).</summary>
        Flat,

        /// <summary>가산 백분율. 같은 스탯의 값들을 합산한 뒤 한 번 적용 (+15%들을 합산).</summary>
        PercentAdd,

        /// <summary>곱연산 백분율. 각각 (1 + value)로 곱한다 (×1.2).</summary>
        PercentMult,
    }
}
