using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EZNEW.DataValidation.Validators
{
    /// <summary>
    /// Compress file validator
    /// </summary>
    public class CompressFileValidator : DataValidator
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
        public override void Validate(dynamic value, string errorMessage)
        {
            SetVerifyResult(ValidationExtensions.IsCompressFileNullable(value?.ToString()), errorMessage);
        }
    }
}
