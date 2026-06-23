namespace GameFramework.World
{
    public interface IWorld
    {
        EntityRegistry EntityRegistry { get; }
        WorldEventBuffer EventBuffer { get; }
        void Tick(long tick, float deltaTime);
    }
}
