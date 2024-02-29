using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Credit card validator
    /// </summary>
    public class CreditCardValidator : SixnetValidator
    {
        /// <summary>
        /// Initialize a credit card validator 
        /// </summary>
        public CreditCardValidator()
        {
            defaultErrorMessageValue = "Incorrect credit card format";
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            var stringValue = value as string;
            return stringValue.IsCreditCardNullable() 
                ? ValidationResult.SuccessResult() 
                : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new CreditCardAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
