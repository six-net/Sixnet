using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System;
using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Remote validator
    /// </summary>
    public class RemoteValidator : SixnetBaseValidator
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
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override SixnetValidationResult Validate(dynamic value, string errorMessage)
        {
            throw new NotImplementedException();
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            return null;
        }
    }
}
