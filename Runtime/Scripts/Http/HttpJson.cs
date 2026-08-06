using Newtonsoft.Json;

namespace GameFramework.Http
{
    public static class HttpJson
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
        };

        public static string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Formatting.Indented, jsonSerializerSettings);
        }

        public static T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, jsonSerializerSettings);
        }
    }
}
