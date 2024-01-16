using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.DataValidation.Validators
{
    /// <summary>
    /// Max length validator
    /// </summary>
    public class MaxLengthValidator : DataValidator
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
        public override void Validate(dynamic value, string errorMessage)
        {
            SetVerifyResult(ValidationExtensions.MaxLength(value, Length), errorMessage);
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
