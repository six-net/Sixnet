// "Company © 2025. All rights reserved."

namespace Sixnet.Validation
{
    /// <summary>
    /// Validator setting
    /// </summary>
    public class SixnetValidatorSetting
    {
        /// <summary>
        /// Gets or sets the validate type
        /// </summary>
        public SixnetValidatorType Type { get; set; }

        /// <summary>
        /// Gets or sets the operator
        /// </summary>
        public SixnetCompareOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the value
        /// </summary>
        public dynamic Value { get; set; }

        /// <summary>
        /// Gets or sets the enum type
        /// </summary>
        public string EnumType { get; set; }

        /// <summary>
        /// Gets or sets the max value
        /// </summary>
        public dynamic MaxValue { get; set; }

        /// <summary>
        /// Gets or sets the min value
        /// </summary>
        public dynamic MinValue { get; set; }

        /// <summary>
        /// Gets or sets the lower boundary
        /// </summary>
        public SixnetRangeBoundary LowerBoundary { get; set; }

        /// <summary>
        /// Gets or sets the upper boundary
        /// </summary>
        public SixnetRangeBoundary UpperBoundary { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the tip message
        /// </summary>
        public bool TipMessage { get; set; }

        /// <summary>
        /// Gets or sets the compare type
        /// </summary>
        public CompareObject CompareType { get; set; }

        /// <summary>
        /// Ignore use scenarios
        /// </summary>
        public List<string> IgnoreUseScenarios { get; set; }
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
