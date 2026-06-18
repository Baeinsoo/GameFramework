namespace GameFramework.World
{
    /// <summary><see cref="Level"/>의 경험치/레벨업 로직. 상태 보유 없음.</summary>
    public class LevelSystem
    {
        /// <summary>경험치를 더하고, ExpToNext를 넘는 동안 레벨업하며 나머지를 이월한다. 증가한 레벨 수를 반환한다.</summary>
        public int AddExperience(Level level, long amount)
        {
            level.Exp += amount;

            int gained = 0;
            while (level.ExpToNext > 0 && level.Exp >= level.ExpToNext)
            {
                level.Exp -= level.ExpToNext;
                level.Value++;
                gained++;
            }

            return gained;
        }

        /// <summary>
        /// 권위 스냅샷 등으로 Value/Exp를 통째로 덮어쓴다. 결정/계산/가드 없음 (Application 메서드).
        /// Level은 상한이 없어 Health/Mana와 달리 클램프하지 않는다(권위 값을 그대로 신뢰).
        /// ExpToNext는 wire 미전송 상수라 건드리지 않는다(생성 시 시드 값 유지).
        /// </summary>
        public void ApplyAuthoritativeState(Level level, int value, long exp)
        {
            level.Value = value;
            level.Exp = exp;
        }
    }
}
