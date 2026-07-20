namespace GameFramework.Rng
{
    /// <summary>결정론 씨앗 조립용 순수 해시 유틸(FNV-1a 64 + 폴딩 combine). 플랫폼 독립 정수 연산.</summary>
    public static class Hashing
    {
        private const ulong Fnv64Offset = 14695981039346656037UL;
        private const ulong Fnv64Prime  = 1099511628211UL;

        /// <summary>문자열의 FNV-1a 64비트 해시(엔티티 id 등 문자열 키 → ulong). null은 빈 문자열과 동일.</summary>
        public static ulong Fnv1a64(string s)
        {
            ulong hash = Fnv64Offset;
            if (s != null)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    hash ^= s[i];
                    hash *= Fnv64Prime;
                }
            }
            return hash;
        }

        /// <summary>누적 해시에 값 하나를 접어 넣는다(씨앗 부품 결합, 순서 의존).</summary>
        // 주의: hash^value 형태(먼저 XOR 후 곱)는 hash와 value가 서로 뒤바뀌어도 값이 같다
        // (XOR는 교환법칙이 성립하므로) — 그러면 Combine(a, b)와 Combine(b, a)가 항상 같아져
        // "순서 의존" 요구를 못 지킨다. 그래서 누적값(hash)을 먼저 곱해 값을 바꾼 다음에
        // value를 섞는다 — 이러면 hash와 value의 역할이 대칭이 아니게 되어 순서가 결과에 반영된다.
        public static ulong Combine(ulong hash, ulong value)
        {
            hash *= Fnv64Prime;
            hash ^= value;
            return hash;
        }
    }
}
