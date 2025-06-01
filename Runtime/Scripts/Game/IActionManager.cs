
namespace GameFramework
{
    public interface IActionManager
    {
        bool TryExecuteAction(IEntity entity, int actionId);
    }

    public interface IActionManager<TEntity> : IActionManager where TEntity : IEntity
    {
        bool TryExecuteAction(TEntity entity, int actionId);
    }
}
