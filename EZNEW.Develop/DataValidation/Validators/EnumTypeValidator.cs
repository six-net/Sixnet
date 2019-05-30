using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// EnumType Validator
    /// </summary>
    public class EnumTypeValidator : DataValidator
    {
        Type _enumType = null;
        public EnumTypeValidator(Type enumType)
        {
            _errorMessage = "value is no longer specified within the enumeration type";
            _enumType = enumType;
        }
        public override void Validate(dynamic value, string errorMessage)
        {
            if (this._enumType == null)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            if (!this._enumType.IsEnum)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            if (value == null)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            string stringValue = value as string;
            if (stringValue != null && String.IsNullOrEmpty(stringValue))
            {
                SetVerifyResult(true, errorMessage);
                return;
            }
            Type valueType = value.GetType();
            if (valueType.IsEnum && this._enumType != valueType)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            if (!valueType.IsValueType && valueType != typeof(string))
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            if (valueType == typeof(bool) || valueType == typeof(float) || valueType == typeof(double) || valueType == typeof(decimal) || valueType == typeof(char))
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            object convertedValue;
            if (valueType.IsEnum)
            {
                convertedValue = value;
            }
            else
            {
                try
                {
                    if (stringValue != null)
                    {
                        convertedValue = Enum.Parse(this._enumType, stringValue, false);
                    }
                    else
                    {
                        convertedValue = Enum.ToObject(this._enumType, value);
                    }
                }
                catch (ArgumentException)
                {
                    SetVerifyResult(false, errorMessage);
                    return;
                }
            }
            if (IsEnumTypeInFlagsMode(this._enumType))
            {
                string underlying = GetUnderlyingTypeValueString(this._enumType, convertedValue);
                string converted = convertedValue.ToString();
                _isValid = !underlying.Equals(converted);
            }
            else
            {
                _isValid = Enum.IsDefined(this._enumType, convertedValue);
            }
            SetVerifyResult(_isValid, errorMessage);

        }
        private static bool IsEnumTypeInFlagsMode(Type enumType)
        {
            return enumType.GetCustomAttributes(typeof(FlagsAttribute), false).Length != 0;
        }
        private static string GetUnderlyingTypeValueString(Type enumType, object enumValue)
        {
            return Convert.ChangeType(enumValue, Enum.GetUnderlyingType(enumType), CultureInfo.InvariantCulture).ToString();
        }

        /// <summary>
        /// Create Validation Attribute
        /// </summary>
        /// <returns></returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new EnumDataTypeAttribute(_enumType)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
