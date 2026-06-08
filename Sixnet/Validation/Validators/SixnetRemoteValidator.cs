// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Remote validator
    /// </summary>
    public class SixnetRemoteValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Create validate attribute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            throw new NotImplementedException();
        }

        public override SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            return null;
        }
    }
}
