using System.Collections.Generic;
using Newtonsoft.Json;

namespace EZNEW.DataValidation.Configuration
{
    /// <summary>
    /// Property validation rule
    /// </summary>
    public class PropertyRule
    {
        /// <summary>
        /// Gets or sets the property name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the rules
        /// </summary>
        [JsonProperty(PropertyName = "rules")]
        public List<ValidatorRule> Rules
        {
            get; set;
        }
    }
}
