using System;
using System.Collections.Concurrent;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using EZNEW.DependencyInjection;
using EZNEW.Serialization.Json;
using EZNEW.Serialization.Json.Newtonsoft;

namespace EZNEW.Serialization
{
    /// <summary>
    /// Json serialize helper
    /// </summary>
    public static class JsonSerializer
    {
        /// <summary>
        /// Json serialization provider
        /// </summary>
        private static readonly IJsonSerializationProvider JsonSerializationProvider = null;

        /// <summary>
        /// Resolve non public options
        /// </summary>
        private static readonly JsonSerializationOptions ResolveNonPublicOptions = new JsonSerializationOptions() { ResolveNonPublic = true };

        private static readonly ConcurrentDictionary<Guid, DataContractJsonSerializer> DataContractJsonSerializers = new ConcurrentDictionary<Guid, DataContractJsonSerializer>();

        static JsonSerializer()
        {
            JsonSerializationProvider = ContainerManager.Resolve<IJsonSerializationProvider>();
            if (JsonSerializationProvider == null)
            {
                JsonSerializationProvider = new JsonNetSerializationProvider();
            }
        }

        #region DataContract json serializer

        /// <summary>
        /// Serialize an object to a json string by DataContract
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="encoding">Encoding,the default is Encoding.UTF8</param>
        /// <returns>Return a json string</returns>
        public static string SerializeByDataContract<T>(T data, Encoding encoding = null)
        {
            if (data == null)
            {
                return string.Empty;
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            var dataType = data?.GetType();
            if (!DataContractJsonSerializers.TryGetValue(dataType.GUID, out var serializer))
            {
                serializer = new DataContractJsonSerializer(dataType);
                DataContractJsonSerializers.TryAdd(dataType.GUID, serializer);
            }
            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, data);
                return encoding.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Deserialize a json string to an object by DataContract
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="json">Json string</param>
        /// <param name="encoding">Encoding,the default is Encoding.UTF8</param>
        /// <returns>Return a data object</returns>
        public static T DeserializeByDataContract<T>(string json, Encoding encoding = null)
        {
            if (string.IsNullOrEmpty(json))
            {
                return default;
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            var dataType = typeof(T);
            if (!DataContractJsonSerializers.TryGetValue(dataType.GUID, out var serializer))
            {
                serializer = new DataContractJsonSerializer(dataType);
                DataContractJsonSerializers.TryAdd(dataType.GUID, serializer);
            }
            var byteValues = encoding.GetBytes(json);
            using (MemoryStream stream = new MemoryStream(byteValues))
            {
                return (T)serializer.ReadObject(stream);
            }
        }

        #endregion

        #region Json Serializer

        /// <summary>
        /// Serialize an object to a json string
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="data">data</param>
        /// <param name="jsonOptions">Json options</param>
        /// <returns>Return a json string</returns>
        public static string Serialize<T>(T data, JsonSerializationOptions jsonOptions)
        {
            return JsonSerializationProvider.Serialize(data, jsonOptions);
        }

        /// <summary>
        /// Serialize an object to a json string
        /// </summary>
        /// <typeparam name="T">data type</typeparam>
        /// <param name="data">data</param>
        /// <param name="resolveNonPublic">Indecates whether resolve non public fields or properties</param>
        /// <returns>Return a json string</returns>
        public static string Serialize<T>(T data, bool resolveNonPublic = false)
        {
            return Serialize(data, resolveNonPublic ? ResolveNonPublicOptions : null);
        }

        /// <summary>
        /// Deserialize a json string to an object
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="jsonOptions">Json options</param>
        /// <returns>Return a data object</returns>
        public static T Deserialize<T>(string json, JsonSerializationOptions jsonOptions)
        {
            return JsonSerializationProvider.Deserialize<T>(json, jsonOptions);
        }

        /// <summary>
        /// Deserialize a json string to an object
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="resolveNonPublic">Indecates whether resolve non public fields or properties</param>
        /// <returns>Return a data object</returns>
        public static T Deserialize<T>(string json, bool resolveNonPublic = false)
        {
            return Deserialize<T>(json, resolveNonPublic ? ResolveNonPublicOptions : null);
        }

        #endregion
    }
}
