using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines modification contract
    /// </summary>
    public interface IModification
    {
        #region Properties

        /// <summary>
        /// Gets all modification entries
        /// </summary>
        IEnumerable<IModificationEntry> ModificationEntries { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Set the modification expression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Modify fields</param>
        /// <returns>Return the newest modification object</returns>
        IModification Set<T>(params Expression<Func<T, dynamic>>[] fields);

        /// <summary>
        /// Set the modification field and value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Set<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Set the modification field name and value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Set(string fieldName, dynamic value);

        /// <summary>
        /// Calculate with current value then modification
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="calculateOperator">Calculate operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Calculate(string fieldName, CalculationOperator calculateOperator, dynamic value);

        /// <summary>
        /// Calculate with current value then modification
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="calculateOperator">Calculate operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Calculate<T>(Expression<Func<T, dynamic>> field, CalculationOperator calculateOperator, dynamic value);

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Add(string fieldName, dynamic value);

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Add<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Subtract(string fieldName, dynamic value);

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Subtract<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Multiply(string fieldName, dynamic value);

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Multiply<T>(Expression<Func<T, dynamic>> field, dynamic value);

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Divide(string fieldName, dynamic value);

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modification object</returns>
        IModification Divide<T>(Expression<Func<T, dynamic>> field, dynamic value);

        #endregion
    }
}
