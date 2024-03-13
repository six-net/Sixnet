using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Compress file validator
    /// </summary>
    public class CompressFileValidator : BaseValidator
    {
        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new FileExtensionsAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage),
                Extensions = ValidationConstants.FileExtensions.CompressFile
            };
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            var success = ValidationExtensions.IsCompressFileNullable(value?.ToString());
            return success 
                ? ValidationResult.SuccessResult() 
                : ValidationResult.ErrorResult(errorMessage);
        }
    }
}
