namespace GameFramework.World
{
    /// <summary><see cref="Stats"/>의 최종값 계산과 모디파이어 관리. 상태 보유 없음.</summary>
    public class StatsSystem
    {
        /// <summary>최종값 = (Base + ΣFlat) × (1 + ΣPercentAdd) × Π(1 + PercentMult). 없는 스탯은 0.</summary>
        public float GetValue(Stats stats, int statType)
        {
            stats.BaseStats.TryGetValue(statType, out var baseValue);

            float flat = 0f;
            float percentAdd = 0f;
            float percentMult = 1f;

            var modifiers = stats.Modifiers;
            for (int i = 0; i < modifiers.Count; i++)
            {
                var modifier = modifiers[i];
                if (modifier.StatType != statType)
                {
                    continue;
                }

                switch (modifier.Type)
                {
                    case ModifierType.Flat:
                        flat += modifier.Value;
                        break;
                    case ModifierType.PercentAdd:
                        percentAdd += modifier.Value;
                        break;
                    case ModifierType.PercentMult:
                        percentMult *= 1f + modifier.Value;
                        break;
                }
            }

            return (baseValue + flat) * (1f + percentAdd) * percentMult;
        }

        public void AddModifier(Stats stats, StatModifier modifier)
        {
            stats.Modifiers.Add(modifier);
        }

        /// <summary>해당 SourceId의 모디파이어를 모두 제거. 하나라도 제거되면 true.</summary>
        public bool RemoveModifiersBySourceId(Stats stats, string sourceId)
        {
            return stats.Modifiers.RemoveAll(modifier => modifier.SourceId == sourceId) > 0;
        }

        /// <summary>베이스 스탯 값을 설정(덮어쓰기)한다. 권위 시드/적용용. 모디파이어는 건드리지 않는다.</summary>
        public void SetBase(Stats stats, int statType, float value)
        {
            stats.BaseStats[statType] = value;
        }

        /// <summary>베이스 스탯에 delta를 더하고(없으면 0 기준) 새 베이스 값을 반환한다. 스탯 포인트 할당용.</summary>
        public float AddBase(Stats stats, int statType, float delta)
        {
            stats.BaseStats.TryGetValue(statType, out var current);
            float next = current + delta;
            stats.BaseStats[statType] = next;
            return next;
        }
    }
}
