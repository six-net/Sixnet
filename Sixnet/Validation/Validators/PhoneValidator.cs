using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Phone validator
    /// </summary>
    public class PhoneValidator : SixnetValidator
    {
        /// <summary>
        /// Initialize a phone validator
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            return SixnetValidationExtensions.IsPhoneNullable(value)
                ? ValidationResult.SuccessResult()
                : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new PhoneAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
