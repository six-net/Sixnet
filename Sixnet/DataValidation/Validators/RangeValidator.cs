using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.DataValidation.Validators
{
    /// <summary>
    /// Range validator
    /// </summary>
    public class RangeValidator : DataValidator
    {
        /// <summary>
        /// Gets the minimum
        /// </summary>
        public object Minimum { get; }

        /// <summary>
        /// Gets the maximum
        /// </summary>
        public object Maximum { get; }

        /// <summary>
        /// Gets the data type
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// Initialize a range validator
        /// </summary>
        /// <param name="dataType">Data type</param>
        /// <param name="minimum">Minimum</param>
        /// <param name="maximum">Maximum</param>
        public RangeValidator(Type dataType, object minimum, object maximum)
        {
            DataType = dataType;
            Minimum = minimum;
            Maximum = maximum;
            defaultErrorMessageValue = "Value out of range";
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            SetVerifyResult(ValidationExtensions.IsInRangeNullable(value, Minimum, Maximum), errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new RangeAttribute(DataType, Minimum?.ToString(), Maximum?.ToString())
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
