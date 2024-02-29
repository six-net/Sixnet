using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using Sixnet.Validation.Validators;
using Sixnet.Expressions.Linq;

namespace Sixnet.Validation
{
    /// <summary>
    /// Default validation
    /// </summary>
    public class DefaultValidation<T> : ISixnetValidation
    {
        #region Fields

        /// <summary>
        /// Field or property access method
        /// </summary>
        readonly Func<T, dynamic> valueMethod = null;

        /// <summary>
        /// Data validator
        /// </summary>
        readonly SixnetValidator validator = null;

        /// <summary>
        /// Field name
        /// </summary>
        readonly string fieldName = string.Empty;

        /// <summary>
        /// Error message
        /// </summary>
        readonly string errorMessage = string.Empty;

        /// <summary>
        /// Compare value
        /// </summary>
        readonly dynamic compareValue = null;

        /// <summary>
        /// Compare property name
        /// </summary>
        readonly string comparePropertyName = string.Empty;

        /// <summary>
        /// Ignore use cases
        /// </summary>
        readonly IEnumerable<string> ignoreUseScenarios = Array.Empty<string>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the Sixnet.Develop.DataValidation.ValidationItem<> class
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="validator">Data validator</param>
        /// <param name="fieldName">Field name</param>
        public DefaultValidation(ValidationField<T> field, SixnetValidator validator, string fieldName)
        {
            valueMethod = field.Field.Compile();
            this.validator = validator;
            this.fieldName = fieldName;
            errorMessage = field.ErrorMessage;
            ignoreUseScenarios = new List<string>(field.IgnoreUseScenarios ?? Array.Empty<string>());
            if (field.CompareValue is Expression<Func<T, dynamic>> compareExp)
            {
                comparePropertyName = SixnetExpressionHelper.GetExpressionText(compareExp);
                compareValue = compareExp.Compile();
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
        public ValidationResult Validate(dynamic data, string useScenario = "")
        {
            if (valueMethod == null || validator == null || IgnoreValidate(useScenario))
            {
                return ValidationResult.SuccessResult();
            }
            dynamic value = valueMethod(data);
            ValidationResult result;
            if (validator is CompareValidator)
            {
                var operatorValue = new CompareVerificationValue()
                {
                    SourceValue = value,
                    CompareValue = compareValue is Func<T, dynamic> ? ((Func<T, dynamic>)compareValue)(data) : compareValue
                };
                result = validator.Validate(operatorValue, errorMessage);
            }
            else
            {
                result = validator.Validate(value, errorMessage);
            }
            return result;
        }

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <returns>Return the validation attribute</returns>
        public ValidationAttribute CreateValidationAttribute()
        {
            if (IgnoreValidate(ValidationConstants.UseCaseNames.Mvc))
            {
                return null;
            }
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

        /// <summary>
        /// Indicates whether innore validate
        /// </summary>
        /// <param name="useScenario">Use scenario</param>
        /// <returns></returns>
        public bool IgnoreValidate(string useScenario)
        {
            if (ignoreUseScenarios.IsNullOrEmpty() || string.IsNullOrWhiteSpace(useScenario))
            {
                return false;
            }
            return ignoreUseScenarios.Any(c => c.Equals(useScenario, StringComparison.OrdinalIgnoreCase));
        }

        #endregion
    }
}
