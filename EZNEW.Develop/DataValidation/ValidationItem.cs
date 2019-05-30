using EZNEW.Develop.DataValidation.Validators;
using EZNEW.Framework.ExpressionUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// Validation Item
    /// </summary>
    public class ValidationItem<T> : IValidation
    {
        public ValidationItem(ValidationField<T> field, DataValidator validator, string fieldName)
        {
            _valueMethod = field.FieldExpression.Compile();
            _validator = validator;
            _fieldName = fieldName;
            _errorMessage = field.ErrorMessage;
            if (field.CompareValue is Expression<Func<T, dynamic>>)
            {
                var compareExpress = (Expression<Func<T, dynamic>>)field.CompareValue;
                _comparePropertyName = ExpressionHelper.GetExpressionText(compareExpress);
                _compareValue = compareExpress.Compile();
            }
            else
            {
                _compareValue = field.CompareValue;
            }
        }

        #region fields

        /// <summary>
        /// get value method
        /// </summary>
        Func<T, dynamic> _valueMethod = null;
        DataValidator _validator = null;
        string _fieldName = string.Empty;
        string _errorMessage = string.Empty;
        dynamic _compareValue = null;
        string _comparePropertyName = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Validate
        /// </summary>
        /// <returns></returns>
        public VerifyResult Validate(dynamic obj)
        {
            if (_valueMethod == null || _validator == null)
            {
                return VerifyResult.SuccessResult();
            }
            dynamic value = _valueMethod(obj);
            if (_validator is CompareValidator)
            {
                CompareOperatorValue operatorValue = new CompareOperatorValue()
                {
                    SourceValue = value
                };
                if (_compareValue is Func<T, dynamic>)
                {
                    operatorValue.CompareValue = ((Func<T, dynamic>)_compareValue)(obj);
                }
                else
                {
                    operatorValue.CompareValue = _compareValue;
                }
                _validator.Validate(operatorValue, _errorMessage);
            }
            else
            {
                _validator.Validate(value, _errorMessage);
            }
            _validator.Result.FieldName = _fieldName;
            return _validator.Result;
        }

        /// <summary>
        /// Create Validation Attribute
        /// </summary>
        /// <returns></returns>
        public ValidationAttribute CreateValidationAttribute()
        {
            if (_validator is CompareValidator)
            {
                if (_compareValue is Func<T, dynamic>)
                {
                    return _validator.CreateValidationAttribute(new ValidationAttributeParameter()
                    {
                        ErrorMessage = _errorMessage,
                        OtherProperty = _comparePropertyName
                    });
                }
                return null;
            }
            else
            {
                return _validator.CreateValidationAttribute(new ValidationAttributeParameter()
                {
                    ErrorMessage = _errorMessage
                });
            }
        }

        #endregion
    }
}
