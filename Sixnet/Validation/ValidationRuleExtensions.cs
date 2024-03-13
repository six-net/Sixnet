using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sixnet.Validation
{
    public static class ValidationRuleExtensions
    {
        #region String length

        /// <summary>
        /// String length
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="maxLength">Max length</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="minLength">Min length</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Length<T>(this ISixnetValidationRule<T> validationRule, int maxLength, string errorMessage = "", int minLength = 0, bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Length(maxLength, minLength, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Email

        /// <summary>
        /// Email
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Email<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Email(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Equal

        /// <summary>
        /// Equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Equal<T>(this ISixnetValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Equal(value, validationRule.Field);
            return validationRule;
        }

        /// <summary>
        /// Equal
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Equal<T>(this ISixnetValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Equal(value, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Not equal

        /// <summary>
        /// NotEqual
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> NotEqual<T>(this ISixnetValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.NotEqual(value, validationRule.Field);
            return validationRule;
        }

        /// <summary>
        /// NotEqual
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> NotEqual<T>(this ISixnetValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.NotEqual(value, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region LessThanOrEqual

        /// <summary>
        /// LessThanOrEqual
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> LessThanOrEqual<T>(this ISixnetValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.LessThanOrEqual(value, validationRule.Field);
            return validationRule;
        }

        /// <summary>
        /// LessThanOrEqual
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> LessThanOrEqual<T>(this ISixnetValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.LessThanOrEqual(value, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region LessThan

        /// <summary>
        /// LessThan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> LessThan<T>(this ISixnetValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.LessThan(value, validationRule.Field);
            return validationRule;
        }

        /// <summary>
        /// LessThan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> LessThan<T>(this ISixnetValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.LessThan(value, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region GreaterThan

        /// <summary>
        /// GreaterThan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> GreaterThan<T>(this ISixnetValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.GreaterThan(value, validationRule.Field);
            return validationRule;
        }

        /// <summary>
        /// GreaterThan
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> GreaterThan<T>(this ISixnetValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.GreaterThan(value, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region GreaterThanOrEqual

        /// <summary>
        /// GreaterThanOrEqual
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> GreaterThanOrEqual<T>(this ISixnetValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.GreaterThanOrEqual(value, validationRule.Field);
            return validationRule;
        }

        /// <summary>
        /// GreaterThanOrEqual
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="value">Value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> GreaterThanOrEqual<T>(this ISixnetValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.GreaterThanOrEqual(value, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region In

        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="values">Values</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> In<T>(this ISixnetValidationRule<T> validationRule, IEnumerable<dynamic> values, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.In(values, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region NotIn

        /// <summary>
        /// In
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="values">Values</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> NotIn<T>(this ISixnetValidationRule<T> validationRule, IEnumerable<dynamic> values, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.NotIn(values, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Enum

        /// <summary>
        /// Enum
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="enumType">Enum type</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> In<T>(this ISixnetValidationRule<T> validationRule, Type enumType, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.EnumType(enumType, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Max length

        /// <summary>
        /// Max length
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="length">Length</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> MaxLength<T>(this ISixnetValidationRule<T> validationRule, int length, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.MaxLength(length, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Min length

        /// <summary>
        /// Min length
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="length">Length</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> MinLength<T>(this ISixnetValidationRule<T> validationRule, int length, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.MinLength(length, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Phone

        /// <summary>
        /// Phone
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Phone<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Phone(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Range

        /// <summary>
        /// Range
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="valueType">Value type</param>
        /// <param name="minValue">Min value</param>
        /// <param name="maxValue">Max value</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Range<T>(this ISixnetValidationRule<T> validationRule, Type valueType, object minValue, object maxValue, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Range(valueType, minValue, maxValue, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Required

        /// <summary>
        /// Required
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Required<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Required(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Url

        /// <summary>
        /// Url
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Url<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Url(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region CreditCard

        /// <summary>
        /// CreditCard
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> CreditCard<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.CreditCard(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region RegularExpression

        /// <summary>
        /// RegularExpression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="pattern">Pattern</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> RegularExpression<T>(this ISixnetValidationRule<T> validationRule, string pattern, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.RegularExpression(pattern, validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Integer

        /// <summary>
        /// Integer
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Integer<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Integer(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region PositiveInteger

        /// <summary>
        /// PositiveInteger
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> PositiveInteger<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.PositiveInteger(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region PositiveIntegerOrZero

        /// <summary>
        /// PositiveIntegerOrZero
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> PositiveIntegerOrZero<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.PositiveIntegerOrZero(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region NegativeInteger

        /// <summary>
        /// NegativeInteger
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> NegativeInteger<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.NegativeInteger(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region NegativeIntegerOrZero

        /// <summary>
        /// NegativeIntegerOrZero
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> NegativeIntegerOrZero<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.NegativeIntegerOrZero(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Fraction

        /// <summary>
        /// Fraction
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Fraction<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Fraction(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region PositiveFraction

        /// <summary>
        /// PositiveFraction
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> PositiveFraction<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.PositiveFraction(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region NegativeFraction

        /// <summary>
        /// NegativeFraction
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> NegativeFraction<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.NegativeFraction(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region PositiveFractionOrZero

        /// <summary>
        /// PositiveFractionOrZero
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> PositiveFractionOrZero<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.PositiveFractionOrZero(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region NegativeFractionOrZero

        /// <summary>
        /// NegativeFractionOrZero
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> NegativeFractionOrZero<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.NegativeFractionOrZero(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Number

        /// <summary>
        /// Number
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Number<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Number(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Color

        /// <summary>
        /// Color
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Color<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Color(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Chinese

        /// <summary>
        /// Chinese
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Chinese<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Chinese(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region PostCode

        /// <summary>
        /// PostCode
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> PostCode<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.PostCode(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Mobile

        /// <summary>
        /// Mobile
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Mobile<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Mobile(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region IPV4

        /// <summary>
        /// IPV4
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> IPV4<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.IPV4(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Date

        /// <summary>
        /// Date
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Date<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Date(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region DateTime

        /// <summary>
        /// DateTime
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> DateTime<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.DateTime(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Letter

        /// <summary>
        /// Letter
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> Letter<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.Letter(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region UpperLetter

        /// <summary>
        /// UpperLetter
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> UpperLetter<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.UpperLetter(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region LowerLetter

        /// <summary>
        /// LowerLetter
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> LowerLetter<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.LowerLetter(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region IdentityCard

        /// <summary>
        /// IdentityCard
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> IdentityCard<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.IdentityCard(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region ImageFile

        /// <summary>
        /// ImageFile
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> ImageFile<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.ImageFile(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region CompressFile

        /// <summary>
        /// CompressFile
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="validationRule">Validation rule</param>
        /// <param name="errorMessage">Error message</param>
        /// <param name="tip">Indicates whether is tip message</param>
        /// <param name="ignoreScenarios">Ignore scenarios</param>
        /// <returns></returns>
        public static ISixnetValidationRule<T> CompressFile<T>(this ISixnetValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            SixnetValidations.CompressFile(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Util

        static void ResetField<T>(ISixnetValidationRule<T> validationRule, string errorMessage, bool tip, IEnumerable<string> ignoreScenarios)
        {
            validationRule.Field.TipMessage = tip;
            validationRule.Field.ErrorMessage = errorMessage;
            validationRule.Field.IgnoreUseScenarios = ignoreScenarios;
        }

        #endregion
    }
}
