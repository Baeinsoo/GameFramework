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
        /// 데미지 결과 이벤트를 그대로 Health에 반영한다. 결정/계산/가드 없음 (Application 메서드).
        /// </summary>
        public void ApplyDamageDealt(Health health, DamageDealtEvent e)
        {
            health.Current = e.remaining;
        }
    }
}
