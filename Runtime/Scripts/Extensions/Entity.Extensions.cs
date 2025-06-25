
namespace GameFramework
{
    public static partial class Extensions
    {
        public static EntityTransform GetEntityTransform(this IEntity entity)
        {
            return new EntityTransform
            {
                position = entity.position,
                rotation = entity.rotation,
                velocity = entity.velocity
            };
        }
    }
}
