using Newtonsoft.Json;
using System.Collections.Generic;

namespace EZNEW.DataValidation.Configuration
{
    /// <summary>
    /// Type validation rule
    /// </summary>
    public class TypeValidationRule
    {
        /// <summary>
        /// Gets or sets the type full name
        /// </summary>
        [JsonProperty(PropertyName = "typeName")]
        public string TypeFullName { get; set; }

        /// <summary>
        /// Gets or set the property rules
        /// </summary>
        [JsonProperty(PropertyName = "properties")]
        public List<PropertyRule> Properties { get; set; }
    }
}
