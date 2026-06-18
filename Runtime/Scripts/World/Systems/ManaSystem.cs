using System;

namespace GameFramework.World
{
    /// <summary><see cref="Mana"/> 데이터를 변경하는 로직. 상태 보유 없음(순수 함수적).</summary>
    public class ManaSystem
    {
        /// <summary>
        /// 권위 스냅샷 등으로 Max/Current를 통째로 덮어쓴다. 결정/계산/가드 없음 (Application 메서드).
        /// Current는 [0, Max]로 클램프해 데이터 무결성만 보장한다.
        /// </summary>
        public void ApplyAuthoritativeState(Mana mana, int max, int current)
        {
            mana.Max = max;
            mana.Current = Math.Clamp(current, 0, max);
        }
    }
}
