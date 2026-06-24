namespace GameFramework.World
{
    public abstract class WorldBase : IWorld
    {
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

        // Generation 페이즈 (world-core-connection-architecture.md). 4b에선 전부 no-op; 4c가 채움.
        protected virtual void Collection(long tick, float deltaTime) { }
        protected virtual void Mutation(long tick, float deltaTime) { }
        protected virtual void Detection(long tick, float deltaTime) { }
    }
}
