using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.DataValidation.Validators
{
    /// <summary>
    /// Min length validator
    /// </summary>
    public class MinLengthValidator : DataValidator
    {
        /// <summary>
        /// Gets the length
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Initialize a min length validator
        /// </summary>
        /// <param name="length">Length</param>
        public MinLengthValidator(int length)
        {
            Length = length;
            defaultErrorMessageValue = "The value is less than the minimum length";
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            SetVerifyResult(ValidationExtensions.MinLength(value, Length), errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new MinLengthAttribute(Length)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
