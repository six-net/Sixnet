using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.DataValidation.Validators
{
    /// <summary>
    /// Required validatory
    /// </summary>
    public class RequiredValidator : DataValidator
    {
        /// <summary>
        /// Initialize a required validator
        /// </summary>
        /// <param name="allowEmptyStrings">Allow empty string</param>
        public RequiredValidator(bool allowEmptyStrings = false)
        {
            AllowEmptyStrings = allowEmptyStrings;
        }

        /// <summary>
        /// Gets or sets whether allow empty value
        /// </summary>
        public bool AllowEmptyStrings { get; set; }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            if (value == null)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            SetVerifyResult(AllowEmptyStrings || !(value is string stringValue) || !string.IsNullOrWhiteSpace(stringValue), errorMessage);
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
                AllowEmptyStrings = AllowEmptyStrings
            };
        }
    }
}
