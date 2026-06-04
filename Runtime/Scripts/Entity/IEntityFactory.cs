namespace GameFramework
{
    public interface IEntityFactory
    {
        TEntity CreateEntity<TEntity, TEntityCreationData>(TEntityCreationData creationData)
            where TEntity : IEntity
            where TEntityCreationData : struct, IEntityCreationData;
    }
}
