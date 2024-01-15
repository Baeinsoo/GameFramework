using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace GameFramework
{
    public static partial class Extensions
    {
        private static BinaryFormatter _formatter;
        public static BinaryFormatter formatter
        {
            get
            {
                if (_formatter == null)
                {
                    SurrogateSelector surrogateSelector = new SurrogateSelector();
                    surrogateSelector.AddSurrogate(typeof(Vector3), new StreamingContext(StreamingContextStates.All), new Vector3SerializationSurrogate());
                    surrogateSelector.AddSurrogate(typeof(Vector2), new StreamingContext(StreamingContextStates.All), new Vector2SerializationSurrogate());

                    _formatter = new BinaryFormatter();
                    _formatter.SurrogateSelector = surrogateSelector;
                }

                return _formatter;
            }
        }

        public static byte[] CompressionSerialize(this object self)
        {
            return ZipByteArray(CommonDataSerialize(self));
        }

        private static byte[] CommonDataSerialize(object customObject)
        {
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, customObject);
                return stream.ToArray();
            }
        }

        private static byte[] ZipByteArray(byte[] data)
        {
            using (var ms = new MemoryStream())
            {
                using (var gs = new GZipStream(ms, CompressionMode.Compress))
                {
                    gs.Write(data, 0, data.Length);
                }
                return ms.ToArray();
            }
        }

        public static object CompressionDeserialize(this byte[] self)
        {
            return CommonDataDeserialize(UnzipByteArray(self));
        }

        private static object CommonDataDeserialize(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return formatter.Deserialize(stream);
            }
        }

        private static byte[] UnzipByteArray(byte[] data)
        {
            using (var resultStream = new MemoryStream())
            {
                using (var sourceStream = new MemoryStream(data))
                {
                    using (var gs = new GZipStream(sourceStream, CompressionMode.Decompress))
                    {
                        gs.CopyTo(resultStream);
                    }
                }
                return resultStream.ToArray();
            }
        }
    }
}
