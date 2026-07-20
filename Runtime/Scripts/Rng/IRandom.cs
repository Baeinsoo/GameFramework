namespace GameFramework.Rng
{
    /// <summary>
    /// 난수 굴림 포트. 시뮬레이션이 구체 RNG(UnityEngine.Random 등)에 직결되지 않도록 주입한다.
    /// 현재 구현(UnityRandom)은 비결정론 — 시드 기반 결정론 구현은 롤백 단계에서 drop-in.
    /// </summary>
    public interface IRandom
    {
        /// <summary>[minInclusive, maxInclusive] 범위의 float. (UnityEngine.Random.Range(float,float) 의미론 — max 포함)</summary>
        float Range(float minInclusive, float maxInclusive);

        /// <summary>[minInclusive, maxExclusive) 범위의 int. (UnityEngine.Random.Range(int,int) 의미론 — max 제외)</summary>
        int Range(int minInclusive, int maxExclusive);
    }
}
