namespace Sixnet.Serialization.Json
{
    public class InternalJsonSerializationProvider : IJsonSerializationProvider
    {
        #region Methods

        /// <summary>
        /// Serialize data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Json serialization options</param>
        /// <returns>Return the json string</returns>
        public string Serialize<T>(T data, JsonSerializationOptions options)
        {
            return System.Text.Json.JsonSerializer.Serialize(data, options?.ConvertToJsonSerializerOptions());
        }

        /// <summary>
        /// Deserialize json
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="options">Json serialization options</param>
        /// <returns>Return object data</returns>
        public T Deserialize<T>(string json, JsonSerializationOptions options)
        {
            return System.Text.Json.JsonSerializer.Deserialize<T>(json, options?.ConvertToJsonSerializerOptions());
        }



        #endregion
    }
}
