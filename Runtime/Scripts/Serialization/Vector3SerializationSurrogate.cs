using UnityEngine;
using System.Runtime.Serialization;

namespace GameFramework
{
    public class Vector3SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector3 vector3 = (Vector3)obj;

            info.AddValue("x", vector3.x);
            info.AddValue("y", vector3.y);
            info.AddValue("z", vector3.z);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            obj = new Vector3(info.GetSingle("x"), info.GetSingle("y"), info.GetSingle("z"));

            return obj;
        }
    }
}
