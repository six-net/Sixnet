using System.Collections.Generic;
using Newtonsoft.Json;

namespace EZNEW.Serialize.Json.JsonNet
{
    /// <summary>
    /// Implement IJsonSerializer using Json.Net
    /// </summary>
    public class JsonNetSerializer : IJsonSerializer
    {
        /// <summary>
        /// json serialize privates resolver
        /// </summary>
        static readonly JsonSerializePrivatesResolver JsonSerializePrivatesResolver = new JsonSerializePrivatesResolver();
        /// <summary>
        /// big number converter
        /// </summary>
        static readonly BigNumberConverter BigNumberConverter = new BigNumberConverter();
        /// <summary>
        /// paging converter
        /// </summary>
        static readonly PagingConverter PagingConverter = new PagingConverter();

        /// <summary>
        /// Serializes a data object as a string
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="data">data</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>json string</returns>
        public string ObjectToJson<T>(T data, JsonSerializeSetting jsonSerializeSetting)
        {
            if (data == null)
            {
                return null;
            }
            if (data.GetType().GUID == typeof(string).GUID)
            {
                return data.ToString();
            }
            var settings = new JsonSerializerSettings();
            settings.Converters.Add(BigNumberConverter);
            if (jsonSerializeSetting != null)
            {
                if (jsonSerializeSetting.ResolveNonPublic)
                {
                    settings.ContractResolver = JsonSerializePrivatesResolver;
                }
            }
            string jsonString = JsonConvert.SerializeObject(data, settings);
            return jsonString;
        }

        /// <summary>
        /// Deserialization a JSON string to an object
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="jsonSerializeSetting">JsonSerializeSetting</param>
        /// <returns>object</returns>
        public T JsonToObject<T>(string json, JsonSerializeSetting jsonSerializeSetting)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }
            if (typeof(T).FullName == typeof(string).FullName)
            {
                return (dynamic)json;
            }
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Converters = new List<JsonConverter>() { PagingConverter },
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            };
            if (jsonSerializeSetting != null)
            {
                if (jsonSerializeSetting.ResolveNonPublic)
                {
                    settings.ContractResolver = JsonSerializePrivatesResolver;
                }
            }
            if (jsonSerializeSetting?.DeserializeType == null)
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            else
            {
                var data = JsonConvert.DeserializeObject(json, jsonSerializeSetting.DeserializeType, settings);
                return (T)data;
            }
        }
    }
}
