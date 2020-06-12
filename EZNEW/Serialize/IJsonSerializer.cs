namespace EZNEW.Serialize
{
    /// <summary>
    /// JSON serializer contract
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serialize the data object as a JSON string
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="data">Data</param>
        /// <param name="jsonSerializeSetting">Json serialize setting</param>
        /// <returns>Return the json string</returns>
        string ObjectToJson<T>(T data, JsonSerializeSetting jsonSerializeSetting);

        /// <summary>
        /// Deserialization a JSON string to an object
        /// </summary>
        /// <param name="json">Json string</param>
        /// <param name="jsonSerializeSetting">Json serialize setting</param>
        /// <returns>Return object data</returns>
        T JsonToObject<T>(string json, JsonSerializeSetting jsonSerializeSetting);
    }
}
