using EZNEW.Develop.DataValidation.Validators;
using EZNEW.Framework.ExpressionUtil;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// Validation Manager
    /// </summary>
    public static class ValidationManager
    {
        static Dictionary<string, Dictionary<string, List<IValidation>>> _typeValidationList = new Dictionary<string, Dictionary<string, List<IValidation>>>();//所有验证配置
        static Dictionary<string, DataValidator> _validatorList = new Dictionary<string, DataValidator>();
        static Dictionary<string, string> defaultValidationTipMsg = new Dictionary<string, string>();

        #region Set Validation

        /// <summary>
        /// Set Validation
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="validator">validator</param>
        /// <param name="fields">fields</param>
        static void SetValidation<T>(DataValidator validator, params ValidationField<T>[] fields)
        {
            if (validator == null || fields == null || fields.Length <= 0)
            {
                return;
            }
            Type sourceType = typeof(T);
            string typeKey = sourceType.FullName;
            Dictionary<string, List<IValidation>> typeValidationItems = null;
            if (_typeValidationList.ContainsKey(typeKey))
            {
                typeValidationItems = _typeValidationList[typeKey];
            }
            else
            {
                typeValidationItems = new Dictionary<string, List<IValidation>>();
                _typeValidationList.Add(typeKey, typeValidationItems);
            }
            foreach (ValidationField<T> property in fields)
            {
                string propertyName = ExpressionHelper.GetExpressionText(property.FieldExpression);
                List<IValidation> validationList = null;
                if (typeValidationItems.ContainsKey(propertyName))
                {
                    validationList = typeValidationItems[propertyName];
                }
                else
                {
                    validationList = new List<IValidation>();
                    typeValidationItems.Add(propertyName, validationList);
                }
                validationList.Add(new ValidationItem<T>(property, validator, propertyName));

                //set tip message
                if (property.TipMessage && !string.IsNullOrWhiteSpace(property.ErrorMessage))
                {
                    string tipKey = string.Format("{0}_{1}", typeKey, propertyName);
                    if (defaultValidationTipMsg.ContainsKey(tipKey))
                    {
                        defaultValidationTipMsg[tipKey] = property.ErrorMessage;
                    }
                    else
                    {
                        defaultValidationTipMsg.Add(tipKey, property.ErrorMessage);
                    }
                }
            }
        }

        /// <summary>
        /// String Length
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="maxLength">max length</param>
        /// <param name="minLength">min length</param>
        /// <param name="fields">fields</param>
        public static void StringLength<T>(int maxLength, int minLength = 0, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}_{2}", typeof(StringLengthValidator).FullName, maxLength, minLength);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new StringLengthValidator(maxLength, minLength);
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// Email
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="fields">fields</param>
        public static void Email<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = typeof(EmailValidator).FullName;
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new EmailValidator();
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// Set Compare Validation
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="compareOperator">compareOperator</param>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void SetCompareValidation<T>(CompareOperator compareOperator, dynamic value, ValidationField<T> field)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(CompareValidator).FullName, (int)compareOperator);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new CompareValidator(compareOperator);
                _validatorList.Add(validatorKey, validator);
            }
            field.CompareValue = value;
            SetValidation(validator, field);
        }

        /// <summary>
        /// Equal
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void Equal<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.Equal, value, field);
        }

        /// <summary>
        /// Equal
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void Equal<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.Equal, value, field);
        }

        /// <summary>
        /// NotEqual
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void NotEqual<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.NotEqual, value, field);
        }

        /// <summary>
        /// NotEqual
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void NotEqual<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.NotEqual, value, field);
        }

        /// <summary>
        /// LessThanOrEqual
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void LessThanOrEqual<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThanOrEqual, value, field);
        }

        /// <summary>
        /// LessThanOrEqual
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void LessThanOrEqual<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThanOrEqual, value, field);
        }

        /// <summary>
        /// LessThan
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void LessThan<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThan, value, field);
        }

        /// <summary>
        /// LessThan
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void LessThan<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThan, value, field);
        }

        /// <summary>
        /// GreaterThan
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void GreaterThan<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThan, value, field);
        }

        /// <summary>
        /// GreaterThan
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void GreaterThan<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThan, value, field);
        }

        /// <summary>
        /// GreaterThanOrEqual
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void GreaterThanOrEqual<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThanOrEqual, value, field);
        }

        /// <summary>
        /// GreaterThanOrEqual
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThanOrEqual, value, field);
        }

        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void In<T>(IEnumerable<dynamic> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.In, value, field);
        }

        /// <summary>
        /// NotIn
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="value">value</param>
        /// <param name="field">field</param>
        public static void NotIn<T>(IEnumerable<dynamic> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.NotIn, value, field);
        }

        /// <summary>
        /// EnumType
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="enumType">enum type</param>
        /// <param name="fields">field</param>
        public static void EnumType<T>(Type enumType, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(EnumTypeValidator).FullName, enumType.FullName);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new EnumTypeValidator(enumType);
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }


        /// <summary>
        /// Max Length
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="length">length</param>
        /// <param name="fields">field</param>
        public static void MaxLength<T>(int length, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(MaxLengthValidator).FullName, length);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new MaxLengthValidator(length);
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// MinLength
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="length">length</param>
        /// <param name="fields">field</param>
        public static void MinLength<T>(int length, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(MinLengthValidator).FullName, length);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new MinLengthValidator(length);
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// Phone
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        public static void Phone<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(PhoneValidator).FullName);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new PhoneValidator();
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// Range
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        public static void Range<T>(dynamic minimum, dynamic maximum, RangeBoundary lowerBoundary = RangeBoundary.Include, RangeBoundary upperBoundary = RangeBoundary.Include, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}_{2}_{3}_{4}", typeof(RangeValidator).FullName, minimum, maximum, (int)lowerBoundary, (int)upperBoundary);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new RangeValidator(minimum, maximum, lowerBoundary, upperBoundary);
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// RegularExpression
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        public static void RegularExpression<T>(string pattern, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(RegularExpressionValidator).FullName, pattern);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new RegularExpressionValidator(pattern);
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        public static void Required<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(RequiredValidator).FullName);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new RequiredValidator();
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// Url
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        public static void Url<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(UrlValidator).FullName);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new UrlValidator();
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        /// <summary>
        /// CreditCard
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        public static void CreditCard<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(CreditCardValidator).FullName);
            DataValidator validator = null;
            if (_validatorList.ContainsKey(validatorKey))
            {
                validator = _validatorList[validatorKey];
            }
            else
            {
                validator = new CreditCardValidator();
                _validatorList.Add(validatorKey, validator);
            }
            SetValidation<T>(validator, fields);
        }

        #endregion

        #region Validate

        /// <summary>
        /// Validate
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="obj">object</param>
        /// <returns></returns>
        public static List<VerifyResult> Validate<T>(T obj)
        {
            string typeName = typeof(T).FullName;
            if (!_typeValidationList.ContainsKey(typeName))
            {
                return new List<VerifyResult>(0);
            }
            Dictionary<string, List<IValidation>> validationList = _typeValidationList[typeName];
            List<VerifyResult> resultList = new List<VerifyResult>();
            foreach (var validation in validationList)
            {
                foreach (var verifyItem in validation.Value)
                {
                    resultList.Add(verifyItem.Validate(obj));
                }
            }
            return resultList;
        }

        #endregion

        #region Get Validation Rules

        /// <summary>
        /// Get Validation Rules
        /// </summary>
        /// <param name="type">DataType</param>
        /// <param name="propertyOrFieldName">property or field name</param>
        /// <returns></returns>
        public static List<IValidation> GetValidationRules(Type type, string propertyOrFieldName)
        {
            if (type == null || string.IsNullOrWhiteSpace(propertyOrFieldName))
            {
                return new List<IValidation>(0);
            }
            if (!_typeValidationList.ContainsKey(type.FullName))
            {
                return new List<IValidation>(0);
            }
            var typeItem = _typeValidationList[type.FullName];
            if (!typeItem.ContainsKey(propertyOrFieldName))
            {
                return new List<IValidation>(0);
            }
            return typeItem[propertyOrFieldName];
        }

        #endregion

        #region Get Validation Tip Message

        /// <summary>
        /// Get Validation Tip Message
        /// </summary>
        /// <param name="type">DataType</param>
        /// <param name="propertyName">property name</param>
        /// <returns></returns>
        public static string GetValidationTipMessage(Type type, string propertyName)
        {
            if (type == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return string.Empty;
            }
            string tipKey = string.Format("{0}_{1}", type.FullName, propertyName);
            if (defaultValidationTipMsg.ContainsKey(tipKey))
            {
                return defaultValidationTipMsg[tipKey];
            }
            return string.Empty;
        }

        /// <summary>
        /// Get Validation Tip Message
        /// </summary>
        /// <typeparam name="T">DataType</typeparam>
        /// <param name="property">property</param>
        /// <returns></returns>
        public static string GetValidationTipMessage<T, TP>(Expression<Func<T, TP>> property)
        {
            if (property == null)
            {
                return string.Empty;
            }
            Type tipType = typeof(T);
            string propertyName = ExpressionHelper.GetExpressionText(property);
            return GetValidationTipMessage(tipType, propertyName);
        }

        #endregion
    }
}
