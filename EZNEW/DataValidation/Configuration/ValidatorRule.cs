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
        public ValidatorType ValidatorType { get; set; }

        /// <summary>
        /// Gets or sets the operator
        /// </summary>
        public CompareOperator Operator { get; set; }

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
        public RangeBoundary LowerBoundary { get; set; }

        /// <summary>
        /// Gets or sets the upper boundary
        /// </summary>
        public RangeBoundary UpperBoundary { get; set; }

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
