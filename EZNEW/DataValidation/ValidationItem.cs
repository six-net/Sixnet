using EZNEW.DataValidation.Validators;
using EZNEW.ExpressionUtil;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;

namespace EZNEW.DataValidation
{
    /// <summary>
    /// Validation item
    /// </summary>
    public class ValidationItem<T> : IValidation
    {
        #region Fields

        /// <summary>
        /// Field or property access method
        /// </summary>
        Func<T, dynamic> valueMethod = null;

        /// <summary>
        /// Data validator
        /// </summary>
        DataValidator validator = null;

        /// <summary>
        /// Field name
        /// </summary>
        string fieldName = string.Empty;

        /// <summary>
        /// Error message
        /// </summary>
        string errorMessage = string.Empty;

        /// <summary>
        /// Compare value
        /// </summary>
        dynamic compareValue = null;

        /// <summary>
        /// Compare property name
        /// </summary>
        string comparePropertyName = string.Empty;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EZNEW.Develop.DataValidation.ValidationItem<> class
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="validator">Data validator</param>
        /// <param name="fieldName">Field name</param>
        public ValidationItem(ValidationField<T> field, DataValidator validator, string fieldName)
        {
            valueMethod = field.Field.Compile();
            this.validator = validator;
            this.fieldName = fieldName;
            errorMessage = field.ErrorMessage;
            if (field.CompareValue is Expression<Func<T, dynamic>> compareExpress)
            {
                comparePropertyName = ExpressionHelper.GetExpressionText(compareExpress);
                compareValue = compareExpress.Compile();
            }
            else
            {
                compareValue = field.CompareValue;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate data
        /// </summary>
        /// <returns>Return the verify result</returns>
        public VerifyResult Validate(dynamic data)
        {
            if (valueMethod == null || validator == null)
            {
                return VerifyResult.SuccessResult();
            }
            dynamic value = valueMethod(data);
            if (validator is CompareValidator)
            {
                CompareVerificationValue operatorValue = new CompareVerificationValue()
                {
                    SourceValue = value
                };
                if (compareValue is Func<T, dynamic>)
                {
                    operatorValue.CompareValue = ((Func<T, dynamic>)compareValue)(data);
                }
                else
                {
                    operatorValue.CompareValue = compareValue;
                }
                validator.Validate(operatorValue, errorMessage);
            }
            else
            {
                validator.Validate(value, errorMessage);
            }
            validator.Result.FieldName = fieldName;
            return validator.Result;
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public ValidationAttribute CreateValidationAttribute()
        {
            if (validator is CompareValidator)
            {
                if (compareValue is Func<T, dynamic>)
                {
                    return validator.CreateValidationAttribute(new ValidationAttributeParameter()
                    {
                        ErrorMessage = errorMessage,
                        OtherProperty = comparePropertyName
                    });
                }
                return null;
            }
            else
            {
                return validator.CreateValidationAttribute(new ValidationAttributeParameter()
                {
                    ErrorMessage = errorMessage
                });
            }
        }

        #endregion
    }
}
