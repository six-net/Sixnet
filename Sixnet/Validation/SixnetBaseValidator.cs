// "Company © 2025. All rights reserved."

using System.ComponentModel.DataAnnotations;

using Sixnet.Localization;

namespace Sixnet.Validation
{
    /// <summary>
    /// Base validator
    /// </summary>
    public abstract class SixnetBaseValidator
    {
        #region Fields

        /// <summary>
        /// Default error message
        /// </summary>
        protected string defaultErrorMessageValue = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default error message
        /// </summary>
        public string DefaultErrorMessage => defaultErrorMessageValue;

        #endregion

        #region Methods

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="parameter">Data</param>
        public abstract SixnetValidationResult Validate(SixnetValidateParameter parameter);

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return the validation attribute</returns>
        public abstract ValidationAttribute CreateValidationAttribute(SixnetValidationAttributeParameter parameter);

        /// <summary>
        /// Format message
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Return the formated message</returns>
        protected string FormatMessage(string errorMessage)
        {
            return string.IsNullOrWhiteSpace(errorMessage) ? defaultErrorMessageValue : errorMessage;
        }

        public virtual SixnetAsyncValidatorRule CreateAsyncValidatorRule(SixnetAsyncValidatorRuleParameter parameter)
        {
            var type = "string";
            if (parameter.FieldType != null)
            {
                var valueType = parameter.FieldType.GetRealValueType();
                switch (Type.GetTypeCode(valueType))
                {
                    case TypeCode.Boolean:
                        type = "boolean";
                        break;
                    case TypeCode.UInt64:
                    case TypeCode.Int64:
                        if (!parameter.LongAsString)
                        {
                            type = "integer";
                        }
                        break;
                    case TypeCode.UInt32:
                    case TypeCode.UInt16:
                    case TypeCode.Byte:
                    case TypeCode.SByte:
                    case TypeCode.Int16:
                    case TypeCode.Int32:
                        type = "integer";
                        break;
                    case TypeCode.Single:
                    case TypeCode.Decimal:
                    case TypeCode.Double:
                        type = "float";
                        break;
                    case TypeCode.DateTime:
                        type = "date";
                        break;
                    case TypeCode.Object:
                        if (valueType.IsArray)
                        {
                            type = "array";
                        }
                        else if (valueType.IsEnum)
                        {
                            type = "enum";
                        }
                        else
                        {
                            type = "object";
                        }
                        break;
                    default:
                        if (valueType == typeof(DateTimeOffset))
                        {
                            type = "date";
                        }
                        break;
                }
            }
            return new SixnetAsyncValidatorRule()
            {
                Message = SixnetLocalizer.GetString(FormatMessage(parameter.ErrorMessage), parameter.MessageArgs?.ToArray()),
                Type = type
            };
        }

        #endregion
    }
}
