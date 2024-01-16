using System.Collections.Generic;
using Sixnet.Serialization.Json.Newtonsoft.Converter;
using Sixnet.Serialization.Json.Newtonsoft.NamingStrategies;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sixnet.Serialization.Json.Newtonsoft
{
    /// <summary>
    /// Implement IJsonSerializationProvider using Json.Net
    /// </summary>
    public class JsonNetSerializationProvider : IJsonSerializationProvider
    {
        #region Fields

        static readonly Dictionary<JsonPropertyNamingPolicy, NamingStrategy> NamingStrategies = new Dictionary<JsonPropertyNamingPolicy, NamingStrategy>()
        {
            { JsonPropertyNamingPolicy.CamelCase,new CamelCaseNamingStrategy(){ OverrideSpecifiedNames=false} },
            { JsonPropertyNamingPolicy.Original,new DefaultNamingStrategy(){ OverrideSpecifiedNames=false} },
            { JsonPropertyNamingPolicy.Uppercase,new UppercaseNamingStrategy(){ OverrideSpecifiedNames=false} },
            { JsonPropertyNamingPolicy.Lowercase,new LowercaseNamingStrategy(){ OverrideSpecifiedNames=false} },
        };

        #endregion

        #region Methods

        /// <summary>
        /// Serializes a data object as a string
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="data">data</param>
        /// <param name="jsonSerializationOptions">Json serialization options</param>
        /// <returns>json string</returns>
        public string Serialize<T>(T data, JsonSerializationOptions jsonSerializationOptions)
        {
            if (data == null)
            {
                return string.Empty; ;
            }
            if (data.GetType().GUID == typeof(string).GUID)
            {
                return data.ToString();
            }
            return JsonConvert.SerializeObject(data, GetJsonSerializerSettings(jsonSerializationOptions));
        }

        /// <summary>
        /// Deserialization a JSON string to an object
        /// </summary>
        /// <param name="json">JSON string</param>
        /// <param name="jsonSerializationOptions">Json serialize setting</param>
        /// <returns>object</returns>
        public T Deserialize<T>(string json, JsonSerializationOptions jsonSerializationOptions)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }
            if (typeof(T).FullName == typeof(string).FullName)
            {
                return (dynamic)json;
            }
            JsonSerializerSettings settings = GetJsonSerializerSettings(jsonSerializationOptions);
            settings.ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            if (jsonSerializationOptions?.DeserializeType == null)
            {
                return JsonConvert.DeserializeObject<T>(json, settings);
            }
            else
            {
                return (T)JsonConvert.DeserializeObject(json, jsonSerializationOptions.DeserializeType, settings);
            }
        }

        static JsonSerializerSettings GetJsonSerializerSettings(JsonSerializationOptions jsonSerializationOptions)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            if (jsonSerializationOptions != null)
            {
                settings.MaxDepth = jsonSerializationOptions.MaxDepth;

                //Resolver
                var contractResolver = GetContractResolver(jsonSerializationOptions);
                //Naming strategy
                contractResolver.NamingStrategy = GetNamingStrategy(jsonSerializationOptions);
                //converters
                if (jsonSerializationOptions.UseCustomizedBigNumberConverter)
                {
                    settings.Converters.Add(BigNumberConverter.Instance);
                }
            }
            return settings;
        }

        static DefaultContractResolver GetContractResolver(JsonSerializationOptions jsonSerializationOptions)
        {
            DefaultContractResolver contractResolver = null;
            if (jsonSerializationOptions.ResolveNonPublic)
            {
                contractResolver = new JsonSerializationPrivatesResolver();
            }
            return contractResolver ?? new DefaultContractResolver();
        }

        static NamingStrategy GetNamingStrategy(JsonSerializationOptions jsonSerializationOptions)
        {
            NamingStrategies.TryGetValue(jsonSerializationOptions?.DefaultNamingPolicy ?? JsonPropertyNamingPolicy.Default, out var namingStrategy);
            return namingStrategy;
        }

        #endregion
    }
}
