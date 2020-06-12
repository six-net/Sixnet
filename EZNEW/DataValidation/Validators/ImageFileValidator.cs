using System;
using System.ComponentModel.DataAnnotations;

namespace EZNEW.DataValidation.Validators
{
    /// <summary>
    /// Image file validator
    /// </summary>
    public class ImageFileValidator : DataValidator
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
                Extensions = ValidationConstants.FileExtensions.ImageFile
            };
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            SetVerifyResult(ValidationExtensions.IsImageFileNullable(value), errorMessage);
        }
    }
}
