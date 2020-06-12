using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EZNEW.Develop.CQuery;
using EZNEW.ExpressionUtil;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// IModify default Implement
    /// </summary>
    [Serializable]
    internal class ModifyExpression : IModify
    {
        #region Fields

        List<IModifyItem> items = new List<IModifyItem>();
        Dictionary<string, IModifyValue> modifyValues = null;

        #endregion

        #region Functions

        /// <summary>
        /// Set the modify expression
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Set<T>(params Expression<Func<T, dynamic>>[] fields)
        {
            if (fields == null || fields.Length < 1)
            {
                return this;
            }
            foreach (var field in fields)
            {
                AddItem(new ExpressionModifyItem(field.Body));
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
        public IModify Set<T>(Expression<Func<T, dynamic>> field, dynamic value)
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
        public IModify Set(string fieldName, dynamic value)
        {
            var fixedValue = new FixedModifyValue(value);
            AddItem(new SetValueModifyItem(fieldName, fixedValue));
            return this;
        }

        /// <summary>
        /// Calculate with current value then modify
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="calculateOperator">calculate operator</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Calculate(string fieldName, CalculateOperator calculateOperator, dynamic value)
        {
            var calculate = new CalculateModifyValue(calculateOperator, value);
            var setValueItem = new SetValueModifyItem(fieldName, calculate);
            AddItem(setValueItem);
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
        public IModify Calculate<T>(Expression<Func<T, dynamic>> field, CalculateOperator calculateOperator, dynamic value)
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
        public IModify Add(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculateOperator.Add, value);
        }

        /// <summary>
        /// Add value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Add<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Add(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Subtract(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculateOperator.Subtract, value);
        }

        /// <summary>
        /// Subtract value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Subtract<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Subtract(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Multiply(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculateOperator.Multiply, value);
        }

        /// <summary>
        /// Multiply value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Multiply<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Multiply(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Divide(string fieldName, dynamic value)
        {
            return Calculate(fieldName, CalculateOperator.Divide, value);
        }

        /// <summary>
        /// Divide value
        /// </summary>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <returns>Return the newest modify object</returns>
        public IModify Divide<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Divide(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// Gets the modify values
        /// </summary>
        /// <returns>Return the modify values</returns>
        public Dictionary<string, IModifyValue> GetModifyValues()
        {
            if (modifyValues != null && modifyValues.Count > 0)
            {
                return modifyValues;
            }
            modifyValues?.Clear();
            modifyValues = modifyValues ?? new Dictionary<string, IModifyValue>();
            foreach (var item in items)
            {
                var itemValue = item.ParseModifyValue();
                if (modifyValues.ContainsKey(itemValue.Key))
                {
                    modifyValues[itemValue.Key] = itemValue.Value;
                }
                else
                {
                    modifyValues.Add(itemValue.Key, itemValue.Value);
                }
            }
            return modifyValues;
        }

        /// <summary>
        /// Add modify item
        /// </summary>
        /// <param name="item">Modify item</param>
        void AddItem(IModifyItem item)
        {
            if (item == null)
            {
                return;
            }
            modifyValues?.Clear();
            items.Add(item);
        }

        #endregion
    }
}
