using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using EZNEW.DependencyInjection;
using EZNEW.Serialize.Json.JsonNet;

namespace EZNEW.Serialize
{
    /// <summary>
    /// Json serialize helper
    /// </summary>
    public static class JsonSerializeHelper
    {
        private static readonly IJsonSerializer JsonSerializer = null;
        private static readonly JsonSerializeSetting ResolveNonPublicSetting = new JsonSerializeSetting() { ResolveNonPublic = true };
        static JsonSerializeHelper()
        {
            JsonSerializer = ContainerManager.Resolve<IJsonSerializer>();
            if (JsonSerializer == null)
            {
                JsonSerializer = new JsonNetSerializer();
            }
        }

        #region DataContractJson Serializer

        /// <summary>
        /// Serialization an object to JSON string by DataContract
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <returns>Return the json string</returns>
        public static string DataContractObjectToJson<T>(T data)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, data);
                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Serialization a json string to an object
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="jsonValue">Json value</param>
        /// <returns>Return the data object</returns>
        public static T JsonToDataContractObject<T>(string jsonValue)
        {
            if (string.IsNullOrEmpty(jsonValue))
            {
                return default;
            }
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            var byteValues = Encoding.UTF8.GetBytes(jsonValue);
            using (MemoryStream stream = new MemoryStream(byteValues))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        #endregion

        #region Json Serializer

        /// <summary>
        /// Serialization an object to JSON string by IJsonSerializer
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="data">data</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>json string</returns>
        public static string ObjectToJson<T>(T data, JsonSerializeSetting jsonSerializeSetting)
        {
            return JsonSerializer.ObjectToJson(data, jsonSerializeSetting);
        }

        /// <summary>
        /// Serialization an object to JSON string by IJsonSerializer
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="data">data</param>
        /// <param name="resolveNonPublic">resolveNonPublic</param>
        /// <returns>Json string</returns>
        public static string ObjectToJson<T>(T data, bool resolveNonPublic = false)
        {
            return ObjectToJson(data, resolveNonPublic ? ResolveNonPublicSetting : null);
        }

        /// <summary>
        /// Deserialization a json string to an object by IJsonSerializer
        /// </summary>
        /// <param name="json">json string</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>data object</returns>
        public static T JsonToObject<T>(string json, JsonSerializeSetting jsonSerializeSetting)
        {
            return JsonSerializer.JsonToObject<T>(json, jsonSerializeSetting);
        }

        /// <summary>
        /// Deserialization a json string to an object by IJsonSerializer
        /// </summary>
        /// <param name="json">json string</param>
        /// <param name="resolveNonPublic">resolveNonPublic</param>
        /// <returns>data object</returns>
        public static T JsonToObject<T>(string json, bool resolveNonPublic = false)
        {
            return JsonToObject<T>(json, resolveNonPublic ? ResolveNonPublicSetting : null);
        }

        #endregion
    }
}
