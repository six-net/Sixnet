using System;
using System.ComponentModel.DataAnnotations;

namespace EZNEW.DataValidation.Validators
{
    /// <summary>
    /// Phone validator
    /// </summary>
    public class PhoneValidator : DataValidator
    {
        /// <summary>
        /// Initialize a phone validator
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            SetVerifyResult(ValidationExtensions.IsPhoneNullable(value), errorMessage);
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
