
namespace GameFramework
{
    public interface IActionManager
    {
        bool TryStartAction(IEntity entity, string actionCode);
        bool TryEndAction(IEntity entity, string actionCode);
    }

    public interface IActionManager<TEntity> : IActionManager where TEntity : IEntity
    {
        bool TryStartAction(TEntity entity, string actionCode);
        bool TryEndAction(TEntity entity, string actionCode);
    }
}
