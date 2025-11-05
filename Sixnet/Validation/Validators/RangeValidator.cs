// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Range validator
    /// </summary>
    public class RangeValidator : SixnetBaseValidator
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
        public override SixnetValidationResult Validate(dynamic value, string errorMessage)
        {
            return ValidationExtensions.IsInRangeNullable(value, Minimum, Maximum)
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new RangeAttribute(DataType, Minimum?.ToString(), Maximum?.ToString())
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Min = Minimum;
            rule.Max = Maximum;

            return rule;
        }
    }
}
