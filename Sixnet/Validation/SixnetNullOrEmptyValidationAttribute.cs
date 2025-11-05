// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation
{
    internal class SixnetNullOrEmptyValidationAttribute : ValidationAttribute
    {
        readonly ValidationAttribute _originalValidationAttribute;
        public SixnetNullOrEmptyValidationAttribute(ValidationAttribute originalValidationAttribute)
        {
            _originalValidationAttribute = originalValidationAttribute;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string stringValue && string.IsNullOrEmpty(stringValue))
            {
                return ValidationResult.Success;
            }
            return _originalValidationAttribute?.GetValidationResult(value, validationContext) ?? ValidationResult.Success;
        }
    }
}
