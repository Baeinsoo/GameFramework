using System;
using System.Linq;
using UnityEngine;

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

        // TODO: 고도화 필요!
        [Obsolete("임시 구현: 고도화 필요!")]
        public static bool IsGrounded(this IEntity entity)
        {
            Vector3 checkPosition = entity.position + Vector3.down * 0.2f;
            Collider[] colliders = Physics.OverlapSphere(checkPosition, 0.4f);

            return colliders.Any(col => col.gameObject.name == "Plane");
        }
    }
}
