using UnityEngine;
using System.Runtime.Serialization;

namespace GameFramework
{
    public class Vector2SerializationSurrogate : ISerializationSurrogate
    {
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            Vector2 vector2 = (Vector2)obj;

            info.AddValue("x", vector2.x);
            info.AddValue("y", vector2.y);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            obj = new Vector2(info.GetSingle("x"), info.GetSingle("y"));

            return obj;
        }
    }
}
