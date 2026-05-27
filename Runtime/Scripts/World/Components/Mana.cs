namespace GameFramework.World
{
    /// <summary>마나(자원) 데이터. 소모/회복 로직은 이후 ManaSystem에 둔다(Anemic).</summary>
    public class Mana : Component
    {
        public int Max { get; set; }
        public int Current { get; set; }

        public Mana(int max)
        {
            Max = max;
            Current = max;
        }
    }
}
