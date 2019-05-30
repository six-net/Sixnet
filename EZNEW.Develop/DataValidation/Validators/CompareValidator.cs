using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// Compare Validator
    /// </summary>
    public class CompareValidator : DataValidator
    {
        CompareOperator _compareOperator = CompareOperator.Equal;
        public CompareValidator(CompareOperator compareOperator)
        {
            _compareOperator = compareOperator;
        }
        public override void Validate(dynamic value, string errorMessage)
        {
            errorMessage = string.IsNullOrWhiteSpace(errorMessage) ? DefaultErrorMessage : errorMessage;
            CompareOperatorValue operatorValue = value as CompareOperatorValue;
            if (operatorValue == null)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            switch (_compareOperator)
            {
                case CompareOperator.Equal:
                default:
                    _isValid = operatorValue.SourceValue == operatorValue.CompareValue;
                    break;
                case CompareOperator.GreaterThan:
                    _isValid = operatorValue.SourceValue > operatorValue.CompareValue;
                    break;
                case CompareOperator.GreaterThanOrEqual:
                    _isValid = operatorValue.SourceValue >= operatorValue.CompareValue;
                    break;
                case CompareOperator.LessThan:
                    _isValid = operatorValue.SourceValue < operatorValue.CompareValue;
                    break;
                case CompareOperator.LessThanOrEqual:
                    _isValid = operatorValue.SourceValue <= operatorValue.CompareValue;
                    break;
                case CompareOperator.NotEqual:
                    _isValid = operatorValue.SourceValue != operatorValue.CompareValue;
                    break;
                case CompareOperator.In:
                    IEnumerable<string> hasCompareValueArray = (operatorValue.CompareValue as IEnumerable<dynamic>).Select<dynamic,string>(c=>c.ToString()).ToList();
                    if (hasCompareValueArray != null)
                    {
                        _isValid = hasCompareValueArray.Any(c => c == operatorValue.SourceValue.ToString());
                    }
                    break;
                case CompareOperator.NotIn:
                    IEnumerable<dynamic> notCompareValueArray = (operatorValue.CompareValue as IEnumerable<dynamic>).Select<dynamic, string>(c => c.ToString()).ToList();
                    if (notCompareValueArray != null)
                    {
                        _isValid = !notCompareValueArray.Any(c => c == operatorValue.SourceValue.ToString());
                    }
                    break;
            }
            SetVerifyResult(_isValid, errorMessage);
        }

        /// <summary>
        /// Create Validation Attribute
        /// </summary>
        /// <returns></returns>
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
