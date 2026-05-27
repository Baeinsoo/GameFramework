using System.Collections.Generic;

namespace GameFramework.World
{
    /// <summary>
    /// 스탯 데이터. 기본값(<see cref="BaseStats"/>)과 모디파이어(<see cref="Modifiers"/>)만 보유한다(Anemic).
    /// 최종값 계산·모디파이어 관리는 <see cref="StatsSystem"/>에 둔다.
    /// </summary>
    public class Stats : Component
    {
        public Dictionary<int, float> BaseStats { get; } = new Dictionary<int, float>();
        public List<StatModifier> Modifiers { get; } = new List<StatModifier>();
    }
}
