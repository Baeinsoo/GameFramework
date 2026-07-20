namespace GameFramework.Rng
{
    /// <summary>UnityEngine.Random으로 <see cref="IRandom"/>을 구현하는 어댑터(비결정론, 전역 상태).</summary>
    public sealed class UnityRandom : IRandom
    {
        public float Range(float minInclusive, float maxInclusive)
            => UnityEngine.Random.Range(minInclusive, maxInclusive);

        public int Range(int minInclusive, int maxExclusive)
            => UnityEngine.Random.Range(minInclusive, maxExclusive);
    }
}
