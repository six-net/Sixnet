using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Enum type validator
    /// </summary>
    public class EnumTypeValidator : SixnetValidator
    {
        /// <summary>
        /// Enum type
        /// </summary>
        readonly Type enumType = null;

        /// <summary>
        /// Initialize a enum validator
        /// </summary>
        /// <param name="enumType">enum type</param>
        public EnumTypeValidator(Type enumType)
        {
            defaultErrorMessageValue = "Value is no longer specified within the enumeration type";
            this.enumType = enumType;
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            return SixnetValidationExtensions.IsEnum(value, enumType)
                ? ValidationResult.SuccessResult()
                : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new EnumDataTypeAttribute(enumType)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
