using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Email validator
    /// </summary>
    public class EmailValidator : BaseValidator
    {
        /// <summary>
        /// Initialize a email validator
        /// </summary>
        public EmailValidator()
        {
            defaultErrorMessageValue = "Incorrect email format";
        }

        /// <summary>
        /// Validate data
        /// </summary>
        /// <param name="value">Validate value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage = "")
        {
            var stringValue = value as string;
            return stringValue.IsEmailNullable()
                ? ValidationResult.SuccessResult()
                : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <param name="parameter">Validation attribute parameter</param>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new EmailAddressAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
