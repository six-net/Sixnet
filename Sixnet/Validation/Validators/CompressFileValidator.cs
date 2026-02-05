// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Expressions.Regular;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Compress file validator
    /// </summary>
    public class CompressFileValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new SixnetNullOrEmptyValidationAttribute(new FileExtensionsAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage),
                Extensions = SixnetValidationConstants.FileExtensions.CompressFile
            });
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter.Value;
            var success = ValidationExtensions.IsCompressFileNullable(value?.ToString());
            return success
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = RegexPatterns.CompressFile;

            return rule;
        }
    }
}
