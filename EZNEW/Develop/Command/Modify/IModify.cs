using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EZNEW.Develop.CQuery;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// Data modify item contract
    /// </summary>
    public interface IModify
    {
        /// <summary>
        /// Set the modify expression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Modify fields</param>
        /// <returns>Return the newest modify object</returns>
        IModify Set<T>(params Expression<Func<T, dynamic>>[] fields);

        /// <summary>
        /// Set the modify field and value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Set<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Set the modify field name and value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Set(string fieldName, dynamic value);

        /// <summary>
        /// Calculate with current value then modify
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="calculateOperator">Calculate operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Calculate(string fieldName, CalculateOperator calculateOperator, dynamic value);

        /// <summary>
        /// Calculate with current value then modify
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="calculateOperator">Calculate operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Calculate<T>(Expression<Func<T, dynamic>> field, CalculateOperator calculateOperator, dynamic value);

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Add(string fieldName, dynamic value);

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Add<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Subtract(string fieldName, dynamic value);

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Subtract<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Multiply(string fieldName, dynamic value);

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Multiply<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Divide(string fieldName, dynamic value);

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        IModify Divide<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Gets the modify values
        /// </summary>
        /// <returns>Return the modify values</returns>
        Dictionary<string, IModifyValue> GetModifyValues();
    }
}
