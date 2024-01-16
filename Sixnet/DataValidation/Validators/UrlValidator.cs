using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.DataValidation.Validators
{
    /// <summary>
    /// Url validator
    /// </summary>
    public class UrlValidator : DataValidator
    {
        /// <summary>
        /// Initialize a url validator
        /// </summary>
        public UrlValidator()
        {
            defaultErrorMessageValue = "Url format error";
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            var valueAsString = value as string;
            SetVerifyResult(ValidationExtensions.IsUrlNullable(value), errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new UrlAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
