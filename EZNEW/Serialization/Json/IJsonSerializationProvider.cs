namespace EZNEW.Serialization.Json
{
    /// <summary>
    /// Defines json serialization provider
    /// </summary>
    public interface IJsonSerializationProvider
    {
        /// <summary>
        /// Serialize data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="options">Json serialization options</param>
        /// <returns>Return the json string</returns>
        string Serialize<T>(T data, JsonSerializationOptions options);

        /// <summary>
        /// Deserialize json
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="options">Json serialization options</param>
        /// <returns>Return object data</returns>
        T Deserialize<T>(string json, JsonSerializationOptions options);
    }
}
