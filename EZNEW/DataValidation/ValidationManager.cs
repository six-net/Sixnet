using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using EZNEW.DataValidation.Configuration;
using EZNEW.DataValidation.Validators;
using EZNEW.ExpressionUtil;
using EZNEW.RegularExpression;
using EZNEW.Serialize;

namespace EZNEW.DataValidation
{
    /// <summary>
    /// Validation manager
    /// </summary>
    public static class ValidationManager
    {
        #region Fields

        /// <summary>
        /// Type validations
        /// Key:type->property
        /// </summary>
        static readonly Dictionary<string, Dictionary<string, List<IValidation>>> TypeValidations = new Dictionary<string, Dictionary<string, List<IValidation>>>();

        /// <summary>
        /// Validators
        /// </summary>
        static readonly Dictionary<string, DataValidator> Validators = new Dictionary<string, DataValidator>();

        /// <summary>
        /// Default validation top message
        /// </summary>
        static readonly Dictionary<string, string> DefaultValidationTipMessage = new Dictionary<string, string>();

        /// <summary>
        /// Field error message separator
        /// </summary>
        public static string FieldErrorMessageSeparator = ":";

        /// <summary>
        /// Enable greed validation mode
        /// </summary>
        public static bool ModelGreedValidation = true;

        #endregion

        #region Configure

        /// <summary>
        /// Configure validation
        /// </summary>
        /// <param name="ruleCollection">Validation rule collection</param>
        public static void Configure(ValidationRuleCollection ruleCollection)
        {
            ruleCollection?.BuildValidation();
        }

        /// <summary>
        /// Configure validation by json data
        /// </summary>
        /// <param name="jsonDatas">Json datas</param>
        public static void ConfigureByJson(params string[] jsonDatas)
        {
            if (jsonDatas.IsNullOrEmpty())
            {
                return;
            }
            foreach (var data in jsonDatas)
            {
                var ruleCollection = JsonSerializeHelper.JsonToObject<ValidationRuleCollection>(data);
                Configure(ruleCollection);
            }
        }

        /// <summary>
        /// Configure validation by config file
        /// </summary>
        /// <param name="configDirectoryPath">Config directory path</param>
        /// <param name="fileExtension">Config file extension</param>
        public static void ConfigureByConfigFile(string configDirectoryPath, string fileExtension = "dvconfig")
        {
            if (string.IsNullOrWhiteSpace(configDirectoryPath) || string.IsNullOrWhiteSpace(fileExtension) || !Directory.Exists(configDirectoryPath))
            {
                return;
            }
            var files = Directory.GetFiles(configDirectoryPath).Where(c => string.Equals(Path.GetExtension(c).Trim('.'), fileExtension, StringComparison.OrdinalIgnoreCase));
            foreach (var file in files)
            {
                var json = File.ReadAllText(file);
                ConfigureByJson(json);
            }
            var childDirectorys = new DirectoryInfo(configDirectoryPath).GetDirectories();
            foreach (var directory in childDirectorys)
            {
                ConfigureByConfigFile(directory.FullName, fileExtension);
            }
        }

        #endregion

        #region Set validation

        /// <summary>
        /// Set validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validator">Validator</param>
        /// <param name="fields">Fields</param>
        static void SetValidation<T>(DataValidator validator, params ValidationField<T>[] fields)
        {
            if (validator == null || fields == null || fields.Length <= 0)
            {
                return;
            }
            Type sourceType = typeof(T);
            string typeKey = sourceType.FullName;
            Dictionary<string, List<IValidation>> typeValidationItems;
            if (TypeValidations.ContainsKey(typeKey))
            {
                typeValidationItems = TypeValidations[typeKey];
            }
            else
            {
                typeValidationItems = new Dictionary<string, List<IValidation>>();
                TypeValidations.Add(typeKey, typeValidationItems);
            }
            foreach (ValidationField<T> property in fields)
            {
                string propertyName = ExpressionHelper.GetExpressionText(property.Field);
                List<IValidation> validationList;
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
                    if (DefaultValidationTipMessage.ContainsKey(tipKey))
                    {
                        DefaultValidationTipMessage[tipKey] = property.ErrorMessage;
                    }
                    else
                    {
                        DefaultValidationTipMessage.Add(tipKey, property.ErrorMessage);
                    }
                }
            }
        }

        /// <summary>
        /// String length
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="maxLength">Max length</param>
        /// <param name="minLength">Min length</param>
        /// <param name="fields">Fields</param>
        public static void StringLength<T>(int maxLength, int minLength = 0, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}_{2}", typeof(StringLengthValidator).FullName, maxLength, minLength);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new StringLengthValidator(maxLength, minLength);
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Email
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Email<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = typeof(EmailValidator).FullName;
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new EmailValidator();
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Set compare validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="compareOperator">Compare operator</param>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void SetCompareValidation<T>(CompareOperator compareOperator, dynamic value, ValidationField<T> field)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(CompareValidator).FullName, (int)compareOperator);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new CompareValidator(compareOperator);
                Validators.Add(validatorKey, validator);
            }
            field.CompareValue = value;
            SetValidation(validator, field);
        }

        /// <summary>
        /// Equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void Equal<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.Equal, value, field);
        }

        /// <summary>
        /// Equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void Equal<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.Equal, value, field);
        }

        /// <summary>
        /// Not equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void NotEqual<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.NotEqual, value, field);
        }

        /// <summary>
        /// Not equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void NotEqual<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.NotEqual, value, field);
        }

        /// <summary>
        /// Lessthan or equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void LessThanOrEqual<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThanOrEqual, value, field);
        }

        /// <summary>
        /// Lessthan or equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void LessThanOrEqual<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThanOrEqual, value, field);
        }

        /// <summary>
        /// Lessthan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void LessThan<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThan, value, field);
        }

        /// <summary>
        /// Lessthan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void LessThan<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.LessThan, value, field);
        }

        /// <summary>
        /// Greaterthan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void GreaterThan<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThan, value, field);
        }

        /// <summary>
        /// Greaterthan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void GreaterThan<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThan, value, field);
        }

        /// <summary>
        /// Greaterthan or equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void GreaterThanOrEqual<T>(dynamic value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThanOrEqual, value, field);
        }

        /// <summary>
        /// Greaterthan or equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void GreaterThanOrEqual<T>(Expression<Func<T, dynamic>> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.GreaterThanOrEqual, value, field);
        }

        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void In<T>(IEnumerable<dynamic> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.In, value, field);
        }

        /// <summary>
        /// Not in
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="value">Value</param>
        /// <param name="field">Field</param>
        public static void NotIn<T>(IEnumerable<dynamic> value, ValidationField<T> field)
        {
            SetCompareValidation(CompareOperator.NotIn, value, field);
        }

        /// <summary>
        /// Enum type
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="enumType">Enum type</param>
        /// <param name="fields">Fields</param>
        public static void EnumType<T>(Type enumType, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(EnumTypeValidator).FullName, enumType.FullName);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new EnumTypeValidator(enumType);
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Max length
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="enumType">Enum type</param>
        /// <param name="fields">Fields</param>
        public static void MaxLength<T>(int length, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(MaxLengthValidator).FullName, length);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new MaxLengthValidator(length);
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Min length
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="enumType">Enum type</param>
        /// <param name="fields">Fields</param>
        public static void MinLength<T>(int length, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(MinLengthValidator).FullName, length);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new MinLengthValidator(length);
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Phone
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        public static void Phone<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(PhoneValidator).FullName);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new PhoneValidator();
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Range
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="valueType">Value type</param>
        /// <param name="minimum">Minimum</param>
        /// <param name="maximum">Maxium</param>
        /// <param name="fields">Fields</param>
        public static void Range<T>(Type valueType, object minimum, object maximum, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}_{2}_{3}", typeof(RangeValidator).FullName, valueType.FullName, minimum, maximum);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new RangeValidator(valueType, minimum, maximum);
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Required
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        public static void Required<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(RequiredValidator).FullName);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new RequiredValidator();
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Url
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        public static void Url<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(UrlValidator).FullName);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new UrlValidator();
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Credit card
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        public static void CreditCard<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}", typeof(CreditCardValidator).FullName);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new CreditCardValidator();
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Regular expression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        public static void RegularExpression<T>(string pattern, params ValidationField<T>[] fields)
        {
            string validatorKey = string.Format("{0}/{1}", typeof(RegularExpressionValidator).FullName, pattern);
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new RegularExpressionValidator(pattern);
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Set integer validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Integer<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.Integer, fields);
        }

        /// <summary>
        /// Set positive integer validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void PositiveInteger<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.PositiveInteger, fields);
        }

        /// <summary>
        /// Set positive integer or zero validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void PositiveIntegerOrZero<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.PositiveIntegerOrZero, fields);
        }

        /// <summary>
        /// Set negative integer validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void NegativeInteger<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.NegativeInteger, fields);
        }

        /// <summary>
        /// Set negative integer or zero validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void NegativeIntegerOrZero<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.NegativeIntegerOrZero, fields);
        }

        /// <summary>
        /// Set fraction validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Fraction<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.Fraction, fields);
        }

        /// <summary>
        /// Set positive fraction validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void PositiveFraction<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.PositiveFraction, fields);
        }

        /// <summary>
        /// Set negative fraction validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void NegativeFraction<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.NegativeFraction, fields);
        }

        /// <summary>
        /// Set positive fraction or zero validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void PositiveFractionOrZero<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.PositiveFractionOrZero, fields);
        }

        /// <summary>
        /// Set negative fraction or zero validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void NegativeFractionOrZero<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.NegativeFractionOrZero, fields);
        }

        /// <summary>
        /// Set number validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Number<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.Number, fields);
        }

        /// <summary>
        /// Set color validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Color<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.Color, fields);
        }

        /// <summary>
        /// Set chinese validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Chinese<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.AllChinese, fields);
        }

        /// <summary>
        /// Set post code validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void PostCode<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.PostCode, fields);
        }

        /// <summary>
        /// Set mobile validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Mobile<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.Mobile, fields);
        }

        /// <summary>
        /// Set ip v4 validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void IPV4<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.IPV4, fields);
        }

        /// <summary>
        /// Set date validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Date<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.Date, fields);
        }

        /// <summary>
        /// Set date time validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void DateTime<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.DateTime, fields);
        }

        /// <summary>
        /// Set letter validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void Letter<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.Letter, fields);
        }

        /// <summary>
        /// Set upper letter validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void UpperLetter<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.UpperLetter, fields);
        }

        /// <summary>
        /// Set lower letter validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void LowerLetter<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.LowerLetter, fields);
        }

        /// <summary>
        /// Set identity card validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void IdentityCard<T>(params ValidationField<T>[] fields)
        {
            RegularExpression(RegexPatterns.IdentityCard, fields);
        }

        /// <summary>
        /// Set image file validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void ImageFile<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = typeof(ImageFileValidator).FullName;
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new ImageFileValidator();
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        /// <summary>
        /// Set compress file validation
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        public static void CompressFile<T>(params ValidationField<T>[] fields)
        {
            string validatorKey = typeof(CompressFileValidator).FullName;
            DataValidator validator;
            if (Validators.ContainsKey(validatorKey))
            {
                validator = Validators[validatorKey];
            }
            else
            {
                validator = new CompressFileValidator();
                Validators.Add(validatorKey, validator);
            }
            SetValidation(validator, fields);
        }

        #endregion

        #region Validate

        /// <summary>
        /// Validate data
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="obj">Object</param>
        /// <returns>Return verify result</returns>
        public static List<VerifyResult> Validate<T>(T obj)
        {
            string typeName = obj?.GetType().FullName;
            if (!TypeValidations.ContainsKey(typeName))
            {
                return new List<VerifyResult>(0);
            }
            Dictionary<string, List<IValidation>> validationList = TypeValidations[typeName];
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

        #region Get validation rules

        /// <summary>
        /// Get data type validation rules
        /// </summary>
        /// <param name="type">Data type</param>
        /// <param name="propertyOrFieldName">Property or field name</param>
        /// <returns>Return the validation rules</returns>
        public static List<IValidation> GetValidationRules(Type type, string propertyOrFieldName)
        {
            if (type == null || string.IsNullOrWhiteSpace(propertyOrFieldName))
            {
                return new List<IValidation>(0);
            }
            if (!TypeValidations.ContainsKey(type.FullName))
            {
                return new List<IValidation>(0);
            }
            var typeItem = TypeValidations[type.FullName];
            if (!typeItem.ContainsKey(propertyOrFieldName))
            {
                return new List<IValidation>(0);
            }
            return typeItem[propertyOrFieldName];
        }

        #endregion

        #region Get validation tip message

        /// <summary>
        /// Get data property validation tip message
        /// </summary>
        /// <param name="type">Data type</param>
        /// <param name="propertyName">Property name</param>
        /// <returns>Return the property tip message</returns>
        public static string GetValidationTipMessage(Type type, string propertyName)
        {
            if (type == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return string.Empty;
            }
            string tipKey = string.Format("{0}_{1}", type.FullName, propertyName);
            if (DefaultValidationTipMessage.ContainsKey(tipKey))
            {
                return DefaultValidationTipMessage[tipKey];
            }
            return string.Empty;
        }

        /// <summary>
        /// Get validation tip message
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <typeparam name="TProperty">Property</typeparam>
        /// <param name="property">Property</param>
        /// <returns>Return the property tip message</returns>
        public static string GetValidationTipMessage<T, TProperty>(Expression<Func<T, TProperty>> property)
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
