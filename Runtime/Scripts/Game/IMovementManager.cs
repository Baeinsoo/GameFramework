
namespace GameFramework
{
    public interface IMovementManager
    {
        void ProcessInput(IEntity entity, float horizontal, float vertical, bool jump);
    }

    public interface IMovementManager<TEntity> : IMovementManager where TEntity : IEntity
    {
        void ProcessInput(TEntity entity, float horizontal, float vertical, bool jump);
    }
}
