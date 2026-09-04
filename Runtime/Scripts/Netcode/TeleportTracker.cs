using System.Collections.Generic;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 엔티티마다 마지막으로 본 텔레포트 카운터를 기억해, 새 값이 그것과 다른지 답한다.
    ///
    /// <para>카운터를 쓰는 이유는 스냅샷이 유실될 수 있어서다 — "이번에 텔레포트함" 같은 일회성
    /// 표시는 그 패킷이 사라지면 영영 관측되지 않지만, 값이 남아 있으면 다음 스냅샷에서 알아챈다.</para>
    /// </summary>
    public class TeleportTracker
    {
        private readonly Dictionary<string, int> _seen = new Dictionary<string, int>();

        /// <summary>이 값이 텔레포트를 뜻하나. 처음 보는 엔티티는 스폰이므로 false다.</summary>
        public bool Observe(string entityId, int count)
        {
            if (_seen.TryGetValue(entityId, out int previous) == false)
            {
                _seen[entityId] = count;
                return false;
            }
            if (previous == count)
            {
                return false;
            }
            _seen[entityId] = count;
            return true;
        }

        public void Forget(string entityId) => _seen.Remove(entityId);

        public void Clear() => _seen.Clear();
    }
}
