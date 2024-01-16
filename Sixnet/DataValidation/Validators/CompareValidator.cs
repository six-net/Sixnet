using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Sixnet.DataValidation.Validators
{
    /// <summary>
    /// Compare validator
    /// </summary>
    public class CompareValidator : DataValidator
    {
        /// <summary>
        /// Compare operator
        /// </summary>
        CompareOperator compareOperator = CompareOperator.Equal;

        /// <summary>
        /// Initialize a new compare validator
        /// </summary>
        /// <param name="compareOperator">Compare operator</param>
        public CompareValidator(CompareOperator compareOperator)
        {
            this.compareOperator = compareOperator;
        }

        /// <summary>
        /// Validate data
        /// </summary>
        /// <param name="value">Validate value</param>
        /// <param name="errorMessage">Error message</param>
        public override void Validate(dynamic value, string errorMessage)
        {
            errorMessage = string.IsNullOrWhiteSpace(errorMessage) ? DefaultErrorMessage : errorMessage;
            CompareVerificationValue compareOperatorValue = value as CompareVerificationValue;
            if (compareOperatorValue == null)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            switch (compareOperator)
            {
                case CompareOperator.Equal:
                default:
                    isValidValue = compareOperatorValue.SourceValue == compareOperatorValue.CompareValue;
                    break;
                case CompareOperator.GreaterThan:
                    isValidValue = compareOperatorValue.SourceValue > compareOperatorValue.CompareValue;
                    break;
                case CompareOperator.GreaterThanOrEqual:
                    isValidValue = compareOperatorValue.SourceValue >= compareOperatorValue.CompareValue;
                    break;
                case CompareOperator.LessThan:
                    isValidValue = compareOperatorValue.SourceValue < compareOperatorValue.CompareValue;
                    break;
                case CompareOperator.LessThanOrEqual:
                    isValidValue = compareOperatorValue.SourceValue <= compareOperatorValue.CompareValue;
                    break;
                case CompareOperator.NotEqual:
                    isValidValue = compareOperatorValue.SourceValue != compareOperatorValue.CompareValue;
                    break;
                case CompareOperator.In:
                    IEnumerable<string> hasCompareValueArray = (compareOperatorValue.CompareValue as IEnumerable<dynamic>).Select<dynamic, string>(c => c.ToString()).ToList();
                    if (hasCompareValueArray != null)
                    {
                        isValidValue = hasCompareValueArray.Any(c => c == compareOperatorValue.SourceValue.ToString());
                    }
                    break;
                case CompareOperator.NotIn:
                    IEnumerable<string> notCompareValueArray = (compareOperatorValue.CompareValue as IEnumerable<dynamic>).Select<dynamic, string>(c => c.ToString()).ToList();
                    if (notCompareValueArray != null)
                    {
                        isValidValue = !notCompareValueArray.Any(c => c == compareOperatorValue.SourceValue.ToString());
                    }
                    break;
            }
            SetVerifyResult(isValidValue, errorMessage);
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            if (compareOperator != CompareOperator.Equal || string.IsNullOrWhiteSpace(parameter.OtherProperty))
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
