using UnityEngine;

namespace GameFramework
{
    public static class MathUtility
    {
        public static Vector3 SmoothDampAngle(Vector3 current, Vector3 target, ref Vector3 currentVelocity, float smoothTime, float maxSpeed, float deltaTime)
        {
            Vector3 rotation;
            rotation.x = Mathf.SmoothDampAngle(current.x, target.x, ref currentVelocity.x, smoothTime, maxSpeed, deltaTime);
            rotation.y = Mathf.SmoothDampAngle(current.y, target.y, ref currentVelocity.y, smoothTime, maxSpeed, deltaTime);
            rotation.z = Mathf.SmoothDampAngle(current.z, target.z, ref currentVelocity.z, smoothTime, maxSpeed, deltaTime);
            return rotation;
        }
    }
}
