// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Expressions.Regular;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Url validator
    /// </summary>
    public class UrlValidator : SixnetBaseValidator
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
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter?.Value;
            var stringValue = value as string;
            return ValidationExtensions.IsUrlNullable(stringValue)
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new SixnetNullOrEmptyValidationAttribute(new UrlAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            });
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = RegexPatterns.Url;

            return rule;
        }
    }
}
