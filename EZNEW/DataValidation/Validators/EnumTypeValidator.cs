using System;
using System.ComponentModel.DataAnnotations;

namespace EZNEW.DataValidation.Validators
{
    /// <summary>
    /// Enum type validator
    /// </summary>
    public class EnumTypeValidator : DataValidator
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
            defaultErrorMessage = "Value is no longer specified within the enumeration type";
            this.enumType = enumType;
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            SetVerifyResult(ValidationExtensions.IsEnumData(value, enumType), errorMessage);
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
