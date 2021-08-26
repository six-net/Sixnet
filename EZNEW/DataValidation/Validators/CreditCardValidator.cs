using System;
using System.ComponentModel.DataAnnotations;

namespace EZNEW.DataValidation.Validators
{
    /// <summary>
    /// Credit card validator
    /// </summary>
    public class CreditCardValidator : DataValidator
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
        public override void Validate(dynamic value, string errorMessage)
        {
            string valueAsString = value as string;
            SetVerifyResult(valueAsString.IsCreditCardNullable(), errorMessage);
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
