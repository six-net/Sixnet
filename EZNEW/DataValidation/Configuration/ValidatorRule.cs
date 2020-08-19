using Newtonsoft.Json;
using System;

namespace EZNEW.DataValidation.Configuration
{
    /// <summary>
    /// Validator rule
    /// </summary>
    public class ValidatorRule
    {
        /// <summary>
        /// Gets or sets the validate type
        /// </summary>
        [JsonProperty(PropertyName = "vtype")]
        public ValidatorType ValidatorType { get; set; }

        /// <summary>
        /// Gets or sets the operator
        /// </summary>
        [JsonProperty(PropertyName = "operator")]
        public CompareOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        [JsonProperty(PropertyName = "value")]
        public dynamic Value { get; set; }

        /// <summary>
        /// Gets or sets the enum type
        /// </summary>
        [JsonProperty(PropertyName = "enumType")]
        public string EnumType { get; set; }

        /// <summary>
        /// Gets or sets the max value
        /// </summary>
        [JsonProperty(PropertyName = "maxValue")]
        public dynamic MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the min value
        /// </summary>
        [JsonProperty(PropertyName = "minValue")]
        public dynamic MinValue { get; set; }

        /// <summary>
        /// Gets or sets the lower boundary
        /// </summary>
        [JsonProperty(PropertyName = "lowerBoundary")]
        public RangeBoundary LowerBoundary { get; set; }

        /// <summary>
        /// Gets or sets the upper boundary
        /// </summary>
        [JsonProperty(PropertyName = "upperBoundary")]
        public RangeBoundary UpperBoundary { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        [JsonProperty(PropertyName = "errorMsg")]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the tip message
        /// </summary>
        [JsonProperty(PropertyName = "tipMsg")]
        public bool TipMessage { get; set; }

        /// <summary>
        /// Gets or sets the compare type
        /// </summary>
        [JsonProperty(PropertyName = "compareType")]
        public CompareObject CompareType { get; set; }
    }

    /// <summary>
    /// Defines compare object
    /// </summary>
    [Serializable]
    public enum CompareObject
    {
        Field,
        Value
    }
}
