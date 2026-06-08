// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Enum type validator
    /// </summary>
    public class SixnetEnumTypeValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Enum type
        /// </summary>
        readonly Type enumType = null;

        /// <summary>
        /// Initialize a enum validator
        /// </summary>
        /// <param name="enumType">enum type</param>
        public SixnetEnumTypeValidator(Type enumType)
        {
            defaultErrorMessageValue = "Value is no longer specified within the enumeration type";
            this.enumType = enumType;
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var value = parameter?.Value;
            return SixnetValidationExtensions.IsEnum(value, enumType)
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(parameter?.ErrorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new EnumDataTypeAttribute(enumType)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        public override SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "enum";
            rule.Enum = Enum.GetValues(enumType);

            return rule;
        }
    }
}
