using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EZNEW.Serialization.Json.Newtonsoft
{
    /// <summary>
    /// Private fields or properties resolver
    /// </summary>
    public class JsonSerializationPrivatesResolver : DefaultContractResolver
    {
        static readonly ConcurrentDictionary<Guid, List<JsonProperty>> CacheJsonProperties = new ConcurrentDictionary<Guid, List<JsonProperty>>();

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            if (CacheJsonProperties.TryGetValue(type.GUID, out List<JsonProperty> jsonPropertys))
            {
                return jsonPropertys ?? new List<JsonProperty>(0);
            }
            jsonPropertys = new List<JsonProperty>();
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Where(c => !typeof(Delegate).IsAssignableFrom(c.FieldType));
            foreach (var field in fields)
            {
                var jsonProperty = base.CreateProperty(field, MemberSerialization.Fields);
                jsonProperty.Writable = true;
                jsonProperty.Readable = true;
                jsonPropertys.Add(jsonProperty);
            }
            CacheJsonProperties[type.GUID] = jsonPropertys;
            return jsonPropertys;
        }

        public JsonSerializationPrivatesResolver Instance = new JsonSerializationPrivatesResolver();
    }
}
