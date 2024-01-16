using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.DataValidation.Validators
{
    /// <summary>
    /// Email validator
    /// </summary>
    public class EmailValidator : DataValidator
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
        public override void Validate(dynamic value, string errorMessage = "")
        {
            string valueAsString = value as string;
            SetVerifyResult(valueAsString.IsEmailNullable(), errorMessage);
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
