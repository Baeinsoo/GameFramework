namespace GameFramework.World
{
    /// <summary>
    /// 레벨/경험치 데이터. 레벨업 로직은 <see cref="LevelSystem"/>에 둔다(Anemic).
    /// <see cref="ExpToNext"/>(다음 레벨까지 필요 경험치)는 외부(레벨 곡선/마스터데이터)에서 설정한다.
    /// </summary>
    public class Level : Component
    {
        public int Value { get; set; }
        public long Exp { get; set; }
        public long ExpToNext { get; set; }
    }
}
