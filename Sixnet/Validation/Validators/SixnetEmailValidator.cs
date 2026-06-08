// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Expressions.Regular;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Email validator
    /// </summary>
    public class SixnetEmailValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Initialize a email validator
        /// </summary>
        public SixnetEmailValidator()
        {
            defaultErrorMessageValue = "Incorrect email format";
        }

        /// <summary>
        /// Validate data
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter?.Value;
            var stringValue = value as string;
            return stringValue.IsEmailNullable()
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <param name="parameter">Validation attribute parameter</param>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new SixnetNullOrEmptyValidationAttribute(new EmailAddressAttribute() { ErrorMessage = FormatMessage(parameter.ErrorMessage) });
        }

        public override SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = SixnetRegexPatterns.Email;

            return rule;
        }
    }
}
