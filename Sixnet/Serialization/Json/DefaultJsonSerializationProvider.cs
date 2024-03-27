using System.Text.Json;

namespace Sixnet.Serialization.Json
{
    internal class DefaultJsonSerializationProvider : IJsonSerializationProvider
    {
        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Json serialization options</param>
        /// <returns>Return the json string</returns>
        public string Serialize<T>(T data, SixnetJsonSerializationOptions options)
        {
            return JsonSerializer.Serialize(data, options?.ConvertToJsonSerializerOptions());
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="options">Json serialization options</param>
        /// <returns>Return object data</returns>
        public T Deserialize<T>(string json, SixnetJsonSerializationOptions options)
        {
            return JsonSerializer.Deserialize<T>(json, options?.ConvertToJsonSerializerOptions());
        }
    }
}
