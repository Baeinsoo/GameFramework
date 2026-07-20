using System;

namespace GameFramework.Netcode
{
    /// <summary>
    /// 틱(=시퀀스 번호)을 키로 최근 N개 항목을 보관하는 고정 크기 링. 슬롯 = tick % capacity로 자리를 잡고,
    /// 병렬 tick 배열로 "그 슬롯이 정말 이 틱인지"를 판별해 덮여 밀려난(stale) 항목을 걸러낸다. 페이로드가
    /// 자기 tick을 들고 있지 않아도 되므로 값형·참조형 모두 안전하다.
    ///
    /// 산업 표준 매핑: Glenn Fiedler의 "sequence buffer"(gafferongames — 넷코드 신뢰성/네트워크 물리).
    /// seq%size 인덱싱 + 병렬 시퀀스 배열로 stale 슬롯을 거르는 구조가 그것. FIFO 큐를 뜻하는 "ring buffer"와 구분.
    /// </summary>
    public class SequenceBuffer<T>
    {
        // 실제 tick(특히 0)과 충돌하지 않는 빈 슬롯 표식.
        private const long EmptyTick = long.MinValue;

        private readonly long[] _ticks;
        private readonly T[] _values;
        private readonly int _capacity;
        private long _latestTick;
        private bool _hasAny;

        public SequenceBuffer(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(capacity));
            }

            _capacity = capacity;
            _ticks = new long[capacity];
            _values = new T[capacity];
            for (int i = 0; i < capacity; i++)
            {
                _ticks[i] = EmptyTick;
            }
        }

        /// <summary>보관 중인 항목 수(용량에서 포화).</summary>
        public int Count { get; private set; }

        /// <summary>가장 최근에 기록된 항목을 돌려준다. 비어 있으면 false.</summary>
        public bool TryGetLatest(out T value)
        {
            if (_hasAny)
            {
                value = _values[Slot(_latestTick)];
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>틱에 항목을 기록한다. 같은 슬롯의 오래된 틱은 덮어써진다.</summary>
        public void Record(long tick, T value)
        {
            int slot = Slot(tick);
            _ticks[slot] = tick;
            _values[slot] = value;
            if (!_hasAny || tick > _latestTick)
            {
                _latestTick = tick;
            }
            _hasAny = true;
            if (Count < _capacity)
            {
                Count++;
            }
        }

        /// <summary>틱으로 항목을 조회한다. 최근 capacity틱 윈도우 밖이거나 미기록이면 false.</summary>
        public bool TryGet(long tick, out T value)
        {
            if (_hasAny && tick <= _latestTick && tick > _latestTick - _capacity)
            {
                int slot = Slot(tick);
                if (_ticks[slot] == tick)
                {
                    value = _values[slot];
                    return true;
                }
            }

            value = default;
            return false;
        }

        private int Slot(long tick) => (int)(((tick % _capacity) + _capacity) % _capacity);
    }
}
