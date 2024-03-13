using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Required validatory
    /// </summary>
    public class RequiredValidator : BaseValidator
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
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            if (value == null)
            {
                return ValidationResult.ErrorResult(errorMessage);
            }
            return (value is string stringValue && (AllowEmptyString || !string.IsNullOrWhiteSpace(stringValue))) || value is not string
                ? ValidationResult.SuccessResult(errorMessage)
                : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new RequiredAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage),
                AllowEmptyStrings = AllowEmptyString
            };
        }
    }
}
