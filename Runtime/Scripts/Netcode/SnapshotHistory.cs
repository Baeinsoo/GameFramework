using System;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 틱을 키로 최근 N틱의 엔티티 상태를 보관하는 링버퍼. 클라 롤백 예측과 서버
    /// lag-compensation이 공유하는 순수 저장소 — "무엇을 언제 기록/복원할지"(정책)는 사이드가 소유한다.
    /// 슬롯 = tick % capacity. 같은 슬롯의 오래된 틱은 새 틱으로 덮여 자동 eviction된다.
    /// </summary>
    public class SnapshotHistory
    {
        // 초기 슬롯이 실제 tick(특히 0)과 충돌하지 않도록 하는 sentinel.
        private const long EmptyTick = long.MinValue;

        private readonly EntitySnapshot[] _ring;
        private readonly int _capacity;
        private long _latestTick;
        private bool _hasAny;

        public SnapshotHistory(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _capacity = capacity;
            _ring = new EntitySnapshot[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _ring[i] = new EntitySnapshot(EmptyTick, default, default, default);
            }
        }

        /// <summary>보관 중인 스냅샷 수(용량에서 포화).</summary>
        public int Count { get; private set; }

        /// <summary>가장 최근에 기록된 스냅샷. 비어 있으면 null.</summary>
        public EntitySnapshot? Latest => _hasAny ? _ring[Slot(_latestTick)] : (EntitySnapshot?)null;

        /// <summary>스냅샷을 기록한다. 같은 슬롯의 오래된 틱은 덮어써진다.</summary>
        public void Record(EntitySnapshot snapshot)
        {
            _ring[Slot(snapshot.Tick)] = snapshot;
            if (!_hasAny || snapshot.Tick > _latestTick)
            {
                _latestTick = snapshot.Tick;
            }
            _hasAny = true;
            if (Count < _capacity)
            {
                Count++;
            }
        }

        /// <summary>틱으로 스냅샷을 조회한다. 최근 capacity틱 윈도우 밖이거나 미기록이면 false.</summary>
        public bool TryGet(long tick, out EntitySnapshot snapshot)
        {
            if (_hasAny && tick <= _latestTick && tick > _latestTick - _capacity)
            {
                var candidate = _ring[Slot(tick)];
                if (candidate.Tick == tick)
                {
                    snapshot = candidate;
                    return true;
                }
            }

            snapshot = default;
            return false;
        }

        private int Slot(long tick) => (int)(((tick % _capacity) + _capacity) % _capacity);
    }
}
