using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sixnet.DataValidation
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
        public static IValidationRule<T> Length<T>(this IValidationRule<T> validationRule, int maxLength, string errorMessage = "", int minLength = 0, bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Length(maxLength, minLength, validationRule.Field);
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
        public static IValidationRule<T> Email<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Email(validationRule.Field);
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
        public static IValidationRule<T> Equal<T>(this IValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Equal(value, validationRule.Field);
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
        public static IValidationRule<T> Equal<T>(this IValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Equal(value, validationRule.Field);
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
        public static IValidationRule<T> NotEqual<T>(this IValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.NotEqual(value, validationRule.Field);
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
        public static IValidationRule<T> NotEqual<T>(this IValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.NotEqual(value, validationRule.Field);
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
        public static IValidationRule<T> LessThanOrEqual<T>(this IValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.LessThanOrEqual(value, validationRule.Field);
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
        public static IValidationRule<T> LessThanOrEqual<T>(this IValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.LessThanOrEqual(value, validationRule.Field);
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
        public static IValidationRule<T> LessThan<T>(this IValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.LessThan(value, validationRule.Field);
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
        public static IValidationRule<T> LessThan<T>(this IValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.LessThan(value, validationRule.Field);
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
        public static IValidationRule<T> GreaterThan<T>(this IValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.GreaterThan(value, validationRule.Field);
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
        public static IValidationRule<T> GreaterThan<T>(this IValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.GreaterThan(value, validationRule.Field);
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
        public static IValidationRule<T> GreaterThanOrEqual<T>(this IValidationRule<T> validationRule, dynamic value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.GreaterThanOrEqual(value, validationRule.Field);
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
        public static IValidationRule<T> GreaterThanOrEqual<T>(this IValidationRule<T> validationRule, Expression<Func<T, dynamic>> value, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.GreaterThanOrEqual(value, validationRule.Field);
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
        public static IValidationRule<T> In<T>(this IValidationRule<T> validationRule, IEnumerable<dynamic> values, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.In(values, validationRule.Field);
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
        public static IValidationRule<T> NotIn<T>(this IValidationRule<T> validationRule, IEnumerable<dynamic> values, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.NotIn(values, validationRule.Field);
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
        public static IValidationRule<T> In<T>(this IValidationRule<T> validationRule, Type enumType, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.EnumType(enumType, validationRule.Field);
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
        public static IValidationRule<T> MaxLength<T>(this IValidationRule<T> validationRule, int length, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.MaxLength(length, validationRule.Field);
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
        public static IValidationRule<T> MinLength<T>(this IValidationRule<T> validationRule, int length, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.MinLength(length, validationRule.Field);
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
        public static IValidationRule<T> Phone<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Phone(validationRule.Field);
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
        public static IValidationRule<T> Range<T>(this IValidationRule<T> validationRule, Type valueType, object minValue, object maxValue, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Range(valueType, minValue, maxValue, validationRule.Field);
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
        public static IValidationRule<T> Required<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Required(validationRule.Field);
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
        public static IValidationRule<T> Url<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Url(validationRule.Field);
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
        public static IValidationRule<T> CreditCard<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.CreditCard(validationRule.Field);
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
        public static IValidationRule<T> RegularExpression<T>(this IValidationRule<T> validationRule, string pattern, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.RegularExpression(pattern, validationRule.Field);
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
        public static IValidationRule<T> Integer<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Integer(validationRule.Field);
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
        public static IValidationRule<T> PositiveInteger<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.PositiveInteger(validationRule.Field);
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
        public static IValidationRule<T> PositiveIntegerOrZero<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.PositiveIntegerOrZero(validationRule.Field);
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
        public static IValidationRule<T> NegativeInteger<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.NegativeInteger(validationRule.Field);
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
        public static IValidationRule<T> NegativeIntegerOrZero<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.NegativeIntegerOrZero(validationRule.Field);
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
        public static IValidationRule<T> Fraction<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Fraction(validationRule.Field);
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
        public static IValidationRule<T> PositiveFraction<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.PositiveFraction(validationRule.Field);
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
        public static IValidationRule<T> NegativeFraction<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.NegativeFraction(validationRule.Field);
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
        public static IValidationRule<T> PositiveFractionOrZero<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.PositiveFractionOrZero(validationRule.Field);
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
        public static IValidationRule<T> NegativeFractionOrZero<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.NegativeFractionOrZero(validationRule.Field);
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
        public static IValidationRule<T> Number<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Number(validationRule.Field);
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
        public static IValidationRule<T> Color<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Color(validationRule.Field);
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
        public static IValidationRule<T> Chinese<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Chinese(validationRule.Field);
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
        public static IValidationRule<T> PostCode<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.PostCode(validationRule.Field);
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
        public static IValidationRule<T> Mobile<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Mobile(validationRule.Field);
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
        public static IValidationRule<T> IPV4<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.IPV4(validationRule.Field);
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
        public static IValidationRule<T> Date<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Date(validationRule.Field);
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
        public static IValidationRule<T> DateTime<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.DateTime(validationRule.Field);
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
        public static IValidationRule<T> Letter<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.Letter(validationRule.Field);
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
        public static IValidationRule<T> UpperLetter<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.UpperLetter(validationRule.Field);
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
        public static IValidationRule<T> LowerLetter<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.LowerLetter(validationRule.Field);
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
        public static IValidationRule<T> IdentityCard<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.IdentityCard(validationRule.Field);
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
        public static IValidationRule<T> ImageFile<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.ImageFile(validationRule.Field);
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
        public static IValidationRule<T> CompressFile<T>(this IValidationRule<T> validationRule, string errorMessage = "", bool tip = false, params string[] ignoreScenarios)
        {
            ResetField(validationRule, errorMessage, tip, ignoreScenarios);
            ValidationManager.CompressFile(validationRule.Field);
            return validationRule;
        }

        #endregion

        #region Util

        static void ResetField<T>(IValidationRule<T> validationRule, string errorMessage, bool tip, IEnumerable<string> ignoreScenarios)
        {
            validationRule.Field.TipMessage = tip;
            validationRule.Field.ErrorMessage = errorMessage;
            validationRule.Field.IgnoreUseScenarios = ignoreScenarios;
        }

        #endregion
    }
}
