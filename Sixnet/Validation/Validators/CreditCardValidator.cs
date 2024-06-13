using System;
using System.ComponentModel.DataAnnotations;
using Sixnet.Expressions.Regular;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Credit card validator
    /// </summary>
    public class CreditCardValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Initialize a credit card validator 
        /// </summary>
        public CreditCardValidator()
        {
            defaultErrorMessageValue = "Incorrect credit card format";
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override SixnetValidationResult Validate(dynamic value, string errorMessage)
        {
            var stringValue = value as string;
            return stringValue.IsCreditCardNullable()
                ? SixnetValidationResult.SuccessResult()
                : SixnetValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            return new SixnetNullOrEmptyValidationAttribute(new CreditCardAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            });
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            var rule = base.CreateAsyncValidatorRule(parameter);

            rule.Type = "string";
            rule.Pattern = RegexPatterns.UnionpayCard;

            return rule;
        }
    }
}
