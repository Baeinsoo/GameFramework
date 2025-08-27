
namespace GameFramework
{
    public interface IEntityController : ICleanup
    {
        IEntity entity { get; }
    }

    public interface IEntityController<T> : IEntityController where T : IEntity
    {
        new T entity { get; }
    }
}
