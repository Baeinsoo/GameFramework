
namespace GameFramework
{
    public interface IActionManager
    {
        bool TryExecuteAction(IEntity entity, string actionCode);
    }

    public interface IActionManager<TEntity> : IActionManager where TEntity : IEntity
    {
        bool TryExecuteAction(TEntity entity, string actionCode);
    }
}
