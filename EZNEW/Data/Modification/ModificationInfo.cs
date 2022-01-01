using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EZNEW.Development.Query;
using EZNEW.Expressions;

namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines modification expression
    /// </summary>
    [Serializable]
    internal class ModificationInfo : IModification
    {
        #region Fields

        readonly List<IModificationEntry> modificationEntries = new List<IModificationEntry>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets all modification entries
        /// </summary>
        public IEnumerable<IModificationEntry> ModificationEntries => modificationEntries;

        #endregion

        #region Functions

        /// <summary>
        /// Set the modify expression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Set<T>(params Expression<Func<T, dynamic>>[] fields)
        {
            if (fields == null || fields.Length < 1)
            {
                return this;
            }
            foreach (var field in fields)
            {
                AddEntry(new ExpressionModificationEntry(field.Body));
            }
            return this;
        }

        /// <summary>
        /// Set the modify field and value
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Set<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            if (value == null || field == null)
            {
                return this;
            }
            return Set(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Set the modify field name and value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Set(string fieldName, dynamic value)
        {
            var fixedValue = new FixedModificationValue(value);
            AddEntry(new SetValueModificationEntry(fieldName, fixedValue));
            return this;
        }

        /// <summary>
        /// Calculate with current value then modify
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="calculateOperator">calculate operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Calculate(string fieldName, CalculationOperator calculateOperator, dynamic value)
        {
            var calculate = new CalculationModificationValue(calculateOperator, value);
            var setValueItem = new SetValueModificationEntry(fieldName, calculate);
            AddEntry(setValueItem);
            return this;
        }

        /// <summary>
        /// Calculate with current value then modify
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="field">Field</param>
        /// <param name="calculateOperator">Calculate operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Calculate<T>(Expression<Func<T, dynamic>> field, CalculationOperator calculateOperator, dynamic value)
        {
            if (value == null)
            {
                return this;
            }
            return Calculate(ExpressionHelper.GetExpressionPropertyName(field.Body), calculateOperator, value);
        }

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Add(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculationOperator.Add, value);
        }

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Add<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Add(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Subtract(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculationOperator.Subtract, value);
        }

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Subtract<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Subtract(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Multiply(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculationOperator.Multiply, value);
        }

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Multiply<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Multiply(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Divide(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculationOperator.Divide, value);
        }

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModification Divide<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Divide(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Add modification entry
        /// </summary>
        /// <param name="entry">Modification entry</param>
        void AddEntry(IModificationEntry entry)
        {
            if (entry != null)
            {
                modificationEntries.Add(entry);
            }
        }

        #endregion
    }
}
