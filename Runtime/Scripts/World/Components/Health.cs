namespace GameFramework.World
{
    /// <summary>체력 데이터. 로직(데미지/회복 등)은 <see cref="HealthSystem"/>에 둔다(Anemic).</summary>
    public class Health : Component
    {
        public int Max { get; set; }
        public int Current { get; set; }

        public bool IsAlive => Current > 0;
        public bool IsDead => !IsAlive;

        public Health(int max)
        {
            Max = max;
            Current = max;
        }
    }
}
