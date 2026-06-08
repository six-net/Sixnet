// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Localization;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Compare validator
    /// </summary>
    public class SixnetCompareValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Compare operator
        /// </summary>
        readonly SixnetCompareOperator _compareOperator = SixnetCompareOperator.Equal;

        /// <summary>
        /// Initialize a new compare validator
        /// </summary>
        /// <param name="compareOperator">Compare operator</param>
        public SixnetCompareValidator(SixnetCompareOperator compareOperator)
        {
            _compareOperator = compareOperator;
        }

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="parameter">Parameter</param>
        public override SixnetValidationResult Validate(SixnetValidateParameter parameter)
        {
            var errorMessage = string.IsNullOrWhiteSpace(parameter?.ErrorMessage) ? DefaultErrorMessage : parameter.ErrorMessage;
            var value = parameter.Value;
            if (value is not SixnetCompareVerificationValue compareValue)
            {
                return SixnetValidationResult.ErrorResult(errorMessage, parameter?.MessageArgs);
            }
            parameter?.MessageArgs.Add(SixnetLocalizer.GetString(_compareOperator.GetEnumName()));
            parameter?.MessageArgs.Add(SixnetLocalizer.GetString(compareValue.SourceValue.ToString()));
            parameter?.MessageArgs.Add(SixnetLocalizer.GetString(compareValue.CompareValue.ToString()));
            var isValid = false;
            switch (_compareOperator)
            {
                case SixnetCompareOperator.Equal:
                default:
                    isValid = compareValue.SourceValue == compareValue.CompareValue;
                    break;
                case SixnetCompareOperator.GreaterThan:
                    isValid = compareValue.SourceValue > compareValue.CompareValue;
                    break;
                case SixnetCompareOperator.GreaterThanOrEqual:
                    isValid = compareValue.SourceValue >= compareValue.CompareValue;
                    break;
                case SixnetCompareOperator.LessThan:
                    isValid = compareValue.SourceValue < compareValue.CompareValue;
                    break;
                case SixnetCompareOperator.LessThanOrEqual:
                    isValid = compareValue.SourceValue <= compareValue.CompareValue;
                    break;
                case SixnetCompareOperator.NotEqual:
                    isValid = compareValue.SourceValue != compareValue.CompareValue;
                    break;
                case SixnetCompareOperator.In:
                    IEnumerable<string> hasCompareValueArray = (compareValue.CompareValue as IEnumerable<dynamic>).Select<dynamic, string>(c => c.ToString()).ToList();
                    if (hasCompareValueArray != null)
                    {
                        isValid = hasCompareValueArray.Any(c => c == compareValue.SourceValue.ToString());
                    }
                    break;
                case SixnetCompareOperator.NotIn:
                    IEnumerable<string> notCompareValueArray = (compareValue.CompareValue as IEnumerable<dynamic>).Select<dynamic, string>(c => c.ToString()).ToList();
                    if (notCompareValueArray != null)
                    {
                        isValid = !notCompareValueArray.Any(c => c == compareValue.SourceValue.ToString());
                    }
                    break;
            }
            return isValid ? SixnetValidationResult.SuccessResult() : SixnetValidationResult.ErrorResult(errorMessage, parameter?.MessageArgs);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter)
        {
            if (_compareOperator != SixnetCompareOperator.Equal || string.IsNullOrWhiteSpace(parameter.OtherProperty))
            {
                return null;
            }
            return new CompareAttribute(parameter.OtherProperty)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        public override SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            return null;
        }
    }
}
