namespace GameFramework.Netcode
{
    /// <summary>
    /// 틱을 키로 최근 N틱의 엔티티 상태를 보관하는 롤백 히스토리. 클라 롤백 예측과 서버 lag-compensation이
    /// 공유하는 순수 저장소 — "무엇을 언제 기록/복원할지"(정책)는 사이드가 소유한다. 링 저장 자체는
    /// <see cref="SequenceBuffer{T}"/>에 위임하고, 여기선 EntitySnapshot 전용 편의(스냅샷이 자기 Tick을 들고
    /// 있어 tick 인자 없이 Record) API만 얹는다.
    /// </summary>
    public class SnapshotHistory
    {
        private readonly SequenceBuffer<EntitySnapshot> _buffer;

        public SnapshotHistory(int capacity)
        {
            _buffer = new SequenceBuffer<EntitySnapshot>(capacity);
        }

        /// <summary>보관 중인 스냅샷 수(용량에서 포화).</summary>
        public int Count => _buffer.Count;

        /// <summary>가장 최근에 기록된 스냅샷. 비어 있으면 null.</summary>
        public EntitySnapshot? Latest => _buffer.TryGetLatest(out var snapshot) ? snapshot : (EntitySnapshot?)null;

        /// <summary>스냅샷을 기록한다. 같은 슬롯의 오래된 틱은 덮어써진다.</summary>
        public void Record(EntitySnapshot snapshot) => _buffer.Record(snapshot.Tick, snapshot);

        /// <summary>틱으로 스냅샷을 조회한다. 최근 capacity틱 윈도우 밖이거나 미기록이면 false.</summary>
        public bool TryGet(long tick, out EntitySnapshot snapshot) => _buffer.TryGet(tick, out snapshot);
    }
}
