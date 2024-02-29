using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Max length validator
    /// </summary>
    public class MaxLengthValidator : SixnetValidator
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
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            return SixnetValidationExtensions.MaxLength(value, Length)
                ? ValidationResult.SuccessResult()
                : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new MaxLengthAttribute(Length)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
