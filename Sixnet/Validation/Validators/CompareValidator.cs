// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Localization;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Compare validator
    /// </summary>
    public class CompareValidator : SixnetBaseValidator
    {
        /// <summary>
        /// Compare operator
        /// </summary>
        readonly CompareOperator _compareOperator = CompareOperator.Equal;

        /// <summary>
        /// Initialize a new compare validator
        /// </summary>
        /// <param name="compareOperator">Compare operator</param>
        public CompareValidator(CompareOperator compareOperator)
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
            if (value is not CompareVerificationValue compareValue)
            {
                return SixnetValidationResult.ErrorResult(errorMessage, parameter?.MessageArgs);
            }
            parameter?.MessageArgs.Add(SixnetLocalizer.GetString(_compareOperator.GetEnumName()));
            parameter?.MessageArgs.Add(SixnetLocalizer.GetString(compareValue.SourceValue.ToString()));
            parameter?.MessageArgs.Add(SixnetLocalizer.GetString(compareValue.CompareValue.ToString()));
            var isValid = false;
            switch (_compareOperator)
            {
                case CompareOperator.Equal:
                default:
                    isValid = compareValue.SourceValue == compareValue.CompareValue;
                    break;
                case CompareOperator.GreaterThan:
                    isValid = compareValue.SourceValue > compareValue.CompareValue;
                    break;
                case CompareOperator.GreaterThanOrEqual:
                    isValid = compareValue.SourceValue >= compareValue.CompareValue;
                    break;
                case CompareOperator.LessThan:
                    isValid = compareValue.SourceValue < compareValue.CompareValue;
                    break;
                case CompareOperator.LessThanOrEqual:
                    isValid = compareValue.SourceValue <= compareValue.CompareValue;
                    break;
                case CompareOperator.NotEqual:
                    isValid = compareValue.SourceValue != compareValue.CompareValue;
                    break;
                case CompareOperator.In:
                    IEnumerable<string> hasCompareValueArray = (compareValue.CompareValue as IEnumerable<dynamic>).Select<dynamic, string>(c => c.ToString()).ToList();
                    if (hasCompareValueArray != null)
                    {
                        isValid = hasCompareValueArray.Any(c => c == compareValue.SourceValue.ToString());
                    }
                    break;
                case CompareOperator.NotIn:
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
            if (_compareOperator != CompareOperator.Equal || string.IsNullOrWhiteSpace(parameter.OtherProperty))
            {
                return null;
            }
            return new CompareAttribute(parameter.OtherProperty)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        public override AsyncValidatorRule CreateAsyncValidatorRule(AsyncValidatorRuleParameter parameter)
        {
            return null;
        }
    }
}
