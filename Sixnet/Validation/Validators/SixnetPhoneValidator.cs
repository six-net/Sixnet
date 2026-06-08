// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Expressions.Regular;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Phone validator
    /// </summary>
    public class SixnetPhoneValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Initialize a phone validator
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter?.Value as string;
            return SixnetValidationExtensions.IsPhoneNullable(value)
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new SixnetNullOrEmptyValidationAttribute(new PhoneAttribute() { ErrorMessage = FormatMessage(parameter.ErrorMessage) });
        }

        public override SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = SixnetRegexPatterns.Phone;

            return rule;
        }
    }
}
