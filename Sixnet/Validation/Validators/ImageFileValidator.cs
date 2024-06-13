using System;
using System.ComponentModel.DataAnnotations;
using Sixnet.Expressions.Regular;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Image file validator
    /// </summary>
    public class ImageFileValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new SixnetNullOrEmptyValidationAttribute(new FileExtensionsAttribute() { ErrorMessage = FormatMessage(parameter.ErrorMessage), Extensions = SixnetValidationConstants.FileExtensions.ImageFile });
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override SixnetValidationResult Validate(dynamic value, string errorMessage)
        {
            return ValidationExtensions.IsImageFileNullable(value)
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(errorMessage);
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = RegexPatterns.ImageFie;

            return rule;
        }
    }
}
