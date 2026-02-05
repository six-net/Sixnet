// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Required validatory
    /// </summary>
    public class RequiredValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Initialize a required validator
        /// </summary>
        /// <param name="allowEmptyString">Allow empty string</param>
        public RequiredValidator(bool allowEmptyString = false)
        {
            AllowEmptyString = allowEmptyString;
        }

        /// <summary>
        /// Gets or sets whether allow empty value
        /// </summary>
        public bool AllowEmptyString { get; set; }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter?.Value;
            if (value == null)
            {
                return SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
            }
            return (value is string stringValue && (AllowEmptyString || !string.IsNullOrWhiteSpace(stringValue))) || value is not string
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            var requiredAttr = new RequiredAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage),
                AllowEmptyStrings = AllowEmptyString
            };
            return AllowEmptyString
                ? new SixnetNullOrEmptyValidationAttribute(requiredAttr)
                : requiredAttr;
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            if (!parameter.Required)
            {
                return null;
            }
            var rule = base.CreateAsyncValidatorRule(parameter);
            rule.Required = true;
            return rule;
        }
    }
}
