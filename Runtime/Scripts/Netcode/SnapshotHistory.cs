namespace GameFramework.Netcode
{
    /// <summary>
    /// 틱을 키로 최근 N틱의 엔티티 상태(위치·속도 등)를 보관하는 재사용 가능한 롤백 히스토리 저장소.
    /// 클라 롤백 예측이나 서버 lag-compensation이 필요로 하는 "틱별 스냅샷 보관/조회"를 대신 해주려고
    /// 만든 범용 유틸이다 — "무엇을 언제 기록/복원할지"(정책)는 쓰는 쪽이 정한다. 링 저장 자체는
    /// <see cref="SequenceBuffer{T}"/>에 위임하고, 여기선 EntitySnapshot 전용 편의(스냅샷이 자기 Tick을 들고
    /// 있어 tick 인자 없이 Record) API만 얹는다.
    ///
    /// 현재는 아무 곳에서도 쓰이지 않는다 — LOP 클라는 상태 저장/복원을 <c>IWorld.SaveState</c>/
    /// <c>LoadState</c>(월드가 직접 구현, 위치·속도 + 게임별 데이터를 함께 저장)로 하고, 서버는 애초에
    /// 전체 롤백을 하지 않는다. 이 클래스는 재사용 패키지 타입으로 남겨둔다 — 나중에 "틱별 스냅샷만
    /// 따로" 필요한 자리가 생기면 다시 쓸 수 있다.
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

        /// <summary>
        /// 이 히스토리가 기록한 가장 이른 틱(밀려나도 유지). 조회 실패가 "아직 살지 않은 틱"인지
        /// "밀려난 틱"인지 가를 때 쓴다 — 상세는 <see cref="SequenceBuffer{T}.FirstRecordedTick"/>.
        /// </summary>
        public long? FirstRecordedTick => _buffer.FirstRecordedTick;

        /// <summary>스냅샷을 기록한다. 같은 슬롯의 오래된 틱은 덮어써진다.</summary>
        public void Record(EntitySnapshot snapshot) => _buffer.Record(snapshot.Tick, snapshot);

        /// <summary>틱으로 스냅샷을 조회한다. 최근 capacity틱 윈도우 밖이거나 미기록이면 false.</summary>
        public bool TryGet(long tick, out EntitySnapshot snapshot) => _buffer.TryGet(tick, out snapshot);
    }
}
