namespace GameFramework
{
    /// <summary>
    /// 씨앗 하나로 재현 가능한 결정론적 난수 스트림(SplitMix64). value 타입 — 이벤트별로
    /// 새로 만들어 순서대로 소비한다(예: 한 공격 해소의 회피→크리→크리배수).
    /// 씨앗 유도(매치시드+tick+entity 등 키 해시)는 소비자(예측 전투) 책임 — 여기선 ulong 씨앗만 받는다.
    /// 비결정 전역 RNG가 필요한 곳은 UnityRandom(IRandom) 유지; 이건 결정론 재현용.
    /// </summary>
    public struct DeterministicRandom
    {
        private ulong _state;

        public DeterministicRandom(ulong seed)
        {
            _state = seed;
        }

        /// <summary>다음 64비트 난수(SplitMix64). 스트림을 한 칸 감는다.</summary>
        public ulong NextUInt64()
        {
            _state += 0x9E3779B97F4A7C15UL;
            ulong z = _state;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }

        /// <summary>[0,1) float. 상위 24비트를 2^24로 나눠 부동소수 누적 없이 결정론 유지.</summary>
        public float NextFloat01()
        {
            return (NextUInt64() >> 40) * (1.0f / 16777216.0f); // 1/2^24
        }

        /// <summary>[minInclusive, maxExclusive) float. min==max면 min.</summary>
        public float Range(float minInclusive, float maxExclusive)
        {
            return minInclusive + (maxExclusive - minInclusive) * NextFloat01();
        }

        /// <summary>[minInclusive, maxExclusive) int. maxExclusive<=minInclusive면 minInclusive(no-op).</summary>
        public int Range(int minInclusive, int maxExclusive)
        {
            if (maxExclusive <= minInclusive)
            {
                return minInclusive;
            }
            ulong span = (ulong)((long)maxExclusive - minInclusive);
            return minInclusive + (int)(NextUInt64() % span);
        }
    }
}
