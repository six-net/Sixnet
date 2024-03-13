using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sixnet.Validation.Validators
{
    /// <summary>
    /// Compare validator
    /// </summary>
    public class CompareValidator : BaseValidator
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
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        public override ValidationResult Validate(dynamic value, string errorMessage)
        {
            errorMessage = string.IsNullOrWhiteSpace(errorMessage) ? DefaultErrorMessage : errorMessage;
            if (value is not CompareVerificationValue compareValue)
            {
                return ValidationResult.ErrorResult(errorMessage);
            }
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
            return isValid ? ValidationResult.SuccessResult() : ValidationResult.ErrorResult(errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
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
    }
}
