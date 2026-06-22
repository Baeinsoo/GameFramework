using System;

namespace GameFramework.World
{
    /// <summary><see cref="Health"/> 데이터를 변경하는 로직. 상태 보유 없음(순수 함수적).</summary>
    public class HealthSystem
    {
        public void TakeDamage(Health health, int amount)
        {
            health.Current = Math.Max(0, health.Current - amount);
        }

        public void Heal(Health health, int amount)
        {
            health.Current = Math.Min(health.Max, health.Current + amount);
        }

        public void SetMax(Health health, int value)
        {
            health.Max = value;

            if (health.Current > health.Max)
            {
                health.Current = health.Max;
            }
        }

        /// <summary>
        /// 권위 스냅샷 등으로 Max/Current를 통째로 덮어쓴다. 결정/계산/가드 없음 (Application 메서드).
        /// Current는 [0, Max]로 클램프해 데이터 무결성만 보장한다.
        /// </summary>
        public void ApplyAuthoritativeState(Health health, int max, int current)
        {
            health.Max = max;
            health.Current = Math.Clamp(current, 0, max);
        }
    }
}
