// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Max length validator
    /// </summary>
    public class MaxLengthValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Gets the length
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Initialize a max length validator
        /// </summary>
        /// <param name="length">Value length</param>
        public MaxLengthValidator(int length)
        {
            defaultErrorMessageValue = "The value exceeds the maximum length";
            Length = length;
        }

        /// <summary>
        /// Validate data
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter?.Value;
            return ValidationExtensions.MaxLength(value, Length)
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new MaxLengthAttribute(Length)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            parameter.MessageArgs.Add(Length.ToString());
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Max = Length;

            return rule;
        }
    }
}
