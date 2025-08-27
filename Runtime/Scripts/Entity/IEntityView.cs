
namespace GameFramework
{
    public interface IEntityView : ICleanup
    {
        IEntity entity { get; }
        IEntityController entityController { get; }
    }

    public interface IEntityView<TEntity, TEntityController> : IEntityView
        where TEntity : IEntity
        where TEntityController : IEntityController<TEntity>
    {
        new TEntity entity { get; }
        new TEntityController entityController { get; }
    }
}
