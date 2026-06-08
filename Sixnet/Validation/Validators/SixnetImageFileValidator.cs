// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Expressions.Regular;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Image file validator
    /// </summary>
    public class SixnetImageFileValidator : SixnetBaseValidator
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
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter?.Value as string;
            return SixnetValidationExtensions.IsImageFileNullable(value)
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        public override SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = SixnetRegexPatterns.ImageFie;

            return rule;
        }
    }
}
