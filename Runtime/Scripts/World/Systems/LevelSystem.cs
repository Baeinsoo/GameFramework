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
    }
}
