using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace GameFramework
{
    public static class MasterDataLoader
    {
        public static async Task<List<T>> LoadFromCSV<T>(string relativePath) where T : IMasterData
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentNullException(nameof(relativePath));
            }

            string uri;

#if UNITY_ANDROID && !UNITY_EDITOR
            uri = Path.Combine(Application.streamingAssetsPath, relativePath);
#else
            uri = "file://" + Path.Combine(Application.streamingAssetsPath, relativePath);
#endif
            var www = UnityWebRequest.Get(uri);
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Failed to load CSV from {uri}: {www.error}");
                return new List<T>();
            }

            string csvData = www.downloadHandler.text;
            using (var reader = new StringReader(csvData))
            {
                return ParseCSVFromReader<T>(reader);
            }
        }

        private static List<T> ParseCSVFromReader<T>(TextReader reader) where T : IMasterData
        {
            var result = new List<T>();
            string[] header = reader.ReadLine()?.Split(',')
                .Select(h => h.Trim().ToLower().Trim('\uFEFF')) //  remove BOM
                .ToArray();

            if (header == null)
            {
                return result;
            }

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                      .ToDictionary(p => p.Name.ToLower(), p => p);

            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                string[] values = line.Split(',');
                if (values == null || values.Length != header.Length)
                {
                    continue;
                }

                T instance = Activator.CreateInstance<T>();
                for (int i = 0; i < header.Length; i++)
                {
                    string columnName = header[i].Trim().ToLower();
                    if (properties.TryGetValue(columnName, out PropertyInfo property))
                    {
                        object convertedValue = ConvertValue(values[i], property.PropertyType);
                        property.SetValue(instance, convertedValue);
                    }
                }
                result.Add(instance);
            }

            return result;
        }

        private static object ConvertValue(string value, Type targetType)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return GetDefaultValue(targetType);
            }

            if (targetType == typeof(int)) return int.Parse(value);
            if (targetType == typeof(float)) return float.Parse(value);
            if (targetType == typeof(double)) return double.Parse(value);
            if (targetType == typeof(bool)) return bool.Parse(value);
            if (targetType == typeof(DateTime)) return DateTime.Parse(value);
            if (targetType == typeof(string)) return value;
            if (targetType.IsEnum) return Enum.Parse(targetType, value);
            if (targetType == typeof(Vector2))
            {
                var parts = value.Split('|');
                return new Vector2(
                    float.Parse(parts[0]),
                    float.Parse(parts[1])
                );
            }
            if (targetType == typeof(Vector3))
            {
                var parts = value.Split('|');
                return new Vector3(
                    float.Parse(parts[0]),
                    float.Parse(parts[1]),
                    float.Parse(parts[2])
                );
            }

            return Convert.ChangeType(value, targetType);
        }

        private static object GetDefaultValue(Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }
    }
}
