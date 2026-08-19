using System.Collections.Generic;

namespace GameFramework.World
{
    public abstract class WorldBase : IWorld
    {
        /// <summary>
        /// 되감기 보관 길이. 128틱 ≈ 2.5초 — 이보다 오래된 서버 스냅은 재생 대신 텔레포트로 처리한다.
        /// 게임이 자기 상태를 담을 때도 <b>같은 길이</b>를 써야 한다. 한쪽만 짧으면 되돌리기가 반쪽이 되는데,
        /// 컴파일도 테스트도 그걸 잡아주지 못한다.
        /// </summary>
        protected const int SaveCapacity = 128;

        // 틱 → (엔티티 id → 위치·회전·속도).
        private readonly Netcode.SequenceBuffer<Dictionary<string, Netcode.EntitySnapshot>> _motionFrames
            = new Netcode.SequenceBuffer<Dictionary<string, Netcode.EntitySnapshot>>(SaveCapacity);

        public EntityRegistry EntityRegistry { get; }
        public WorldEventBuffer EventBuffer { get; }

        protected WorldBase(EntityRegistry entityRegistry, WorldEventBuffer eventBuffer)
        {
            EntityRegistry = entityRegistry;
            EventBuffer = eventBuffer;
        }

        public void Tick(long tick, float deltaTime)
        {
            Collection(tick, deltaTime);
            Mutation(tick, deltaTime);
            Detection(tick, deltaTime);
        }

        public long? FirstSavedTick => _motionFrames.FirstRecordedTick;

        public long? LatestSavedTick => _motionFrames.LatestTick;

        public void SaveState(long tick)
        {
            var frame = new Dictionary<string, Netcode.EntitySnapshot>();
            foreach (var entity in EntityRegistry.All)
            {
                if (!entity.Has<Simulated>())
                {
                    continue;   // 시뮬하지 않는 엔티티는 되돌릴 것도 없다(보간으로 따라옴)
                }
                var transform = entity.Get<Transform>();
                var velocity = entity.Get<Velocity>();
                if (transform == null || velocity == null)
                {
                    continue;
                }
                frame[entity.Id] = new Netcode.EntitySnapshot(
                    tick, transform.Position, transform.Rotation, velocity.Linear);
            }
            _motionFrames.Record(tick, frame);

            SaveGameState(tick);
        }

        public bool LoadState(long tick)
        {
            if (!_motionFrames.TryGet(tick, out var frame))
            {
                return false;
            }

            foreach (var pair in frame)
            {
                var entity = EntityRegistry.Get(pair.Key);
                if (entity == null)
                {
                    continue;   // 그 사이 사라진 엔티티 — 되돌릴 대상이 없다
                }
                var transform = entity.Get<Transform>();
                var velocity = entity.Get<Velocity>();
                if (transform == null || velocity == null)
                {
                    continue;
                }
                transform.Position = pair.Value.Position;
                transform.Rotation = pair.Value.Rotation;
                velocity.Linear = pair.Value.Velocity;
            }

            return LoadGameState(tick);
        }

        public bool TryGetSavedMotion(long tick, string entityId, out Netcode.EntitySnapshot motion)
        {
            motion = default;
            return _motionFrames.TryGet(tick, out var frame) && frame.TryGetValue(entityId, out motion);
        }

        /// <summary>
        /// 게임이 자기 상태를 얹는 자리. 베이스는 위치·속도만 담으므로, 스킬·상태이상처럼
        /// 그 게임에만 있는 것은 여기서 담는다. Unreal <c>FSavedMove_Character</c> 서브클래싱과 같은 자리다.
        /// </summary>
        protected virtual void SaveGameState(long tick) { }

        /// <summary>
        /// 게임이 얹은 상태를 되돌린다. 그 틱 기록이 없으면 false.
        /// 여기서 false를 돌려줘도, <see cref="LoadState"/>가 이미 적용한 베이스 위치·속도는
        /// 되돌아가지 않고 그대로 남는다 — 되돌리기 실패를 이유로 베이스 값까지 롤백하지 않는다.
        /// </summary>
        protected virtual bool LoadGameState(long tick) => true;

        // Generation 페이즈 (world-core-connection-architecture.md).
        protected virtual void Collection(long tick, float deltaTime) { }
        protected virtual void Mutation(long tick, float deltaTime) { }
        protected virtual void Detection(long tick, float deltaTime) { }
    }
}
