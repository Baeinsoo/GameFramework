
namespace GameFramework
{
    public interface IMovementManager
    {
        void ProcessInput(IEntity entity, EntityTransform entityTransform, float horizontal, float vertical, bool jump);
    }

    public interface IMovementManager<TEntity> : IMovementManager where TEntity : IEntity
    {
        void ProcessInput(TEntity entity, EntityTransform entityTransform, float horizontal, float vertical, bool jump);
    }
}
