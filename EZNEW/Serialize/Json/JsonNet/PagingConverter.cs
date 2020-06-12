using System;
using EZNEW.Paging;
using Newtonsoft.Json;

namespace EZNEW.Serialize.Json.JsonNet
{
    /// <summary>
    /// Provides a conversion implementation for EZNEW.Paging.Paging<> serialization
    /// </summary>
    public class PagingConverter : JsonConverter
    {
        /// <summary>
        /// Determines whether the specified data type is allowed to be converted
        /// </summary>
        /// <param name="objectType">data type</param>
        /// <returns>allowed to be converted</returns>
        public override bool CanConvert(Type objectType)
        {
            if (!objectType.IsGenericType)
            {
                return false;
            }
            var pagingType = typeof(IPaging<>);
            var genericType = objectType.GetGenericTypeDefinition();
            if (genericType == null)
            {
                return false;
            }
            return genericType == pagingType || pagingType.IsAssignableFrom(genericType);
        }

        /// <summary>
        /// Read the data from the JSON data
        /// </summary>
        /// <param name="reader">Json reader</param>
        /// <param name="objectType">Object type</param>
        /// <param name="existingValue">Existing value</param>
        /// <param name="serializer">Json serializer</param>
        /// <returns>Return object data</returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Type genericType = objectType.GetGenericArguments()[0];
            Type pagingType = typeof(DefaultPaging<>).MakeGenericType(genericType);
            return serializer.Deserialize(reader, pagingType);
        }

        /// <summary>
        /// Write the object data to JSON
        /// </summary>
        /// <param name="writer">Json writer</param>
        /// <param name="value">object value</param>
        /// <param name="serializer">Json serializer</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
