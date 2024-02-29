using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Url validator
    /// </summary>
    public class UrlValidator : SixnetValidator
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
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            var stringValue = value as string;
            return SixnetValidationExtensions.IsUrlNullable(stringValue)
                ? ValidationResult.SuccessResult()
                : ValidationResult.ErrorResult(errorMessage);
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
