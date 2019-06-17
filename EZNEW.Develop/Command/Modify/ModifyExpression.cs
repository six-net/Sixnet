using EZNEW.Develop.CQuery;
using EZNEW.Framework.ExpressionUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// IModify default Implement
    /// </summary>
    internal class ModifyExpression : IModify
    {
        #region fields

        List<IModifyItem> items = new List<IModifyItem>();
        Dictionary<string, IModifyValue> modifyValues = null;

        #endregion

        #region functions

        /// <summary>
        /// set modify expression
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="fields">fields</param>
        /// <returns>IModify object</returns>
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
        /// set modify expression
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Set<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            if (value == null || field == null)
            {
                return this;
            }
            return Set(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// set modify value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Set(string name, dynamic value)
        {
            var fixedValue = new FixedModifyValue(value);
            AddItem(new SetValueModifyItem(name, fixedValue));
            return this;
        }

        /// <summary>
        /// calculate with current value then modify
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="calculateOperator">Calculate Operator</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Calculate(string name, CalculateOperator calculateOperator, dynamic value)
        {
            var calculate = new CalculateModifyValue(calculateOperator,value);
            var setValueItem = new SetValueModifyItem(name, calculate);
            AddItem(setValueItem);
            return this;
        }

        /// <summary>
        /// calculate with current value then modify
        /// </summary>
        /// <typeparam name="T">Model Type</typeparam>
        /// <param name="field">field</param>
        /// <param name="calculateOperator">Calculate Operator</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Calculate<T>(Expression<Func<T, dynamic>> field, CalculateOperator calculateOperator, dynamic value)
        {
            if (value == null)
            {
                return this;
            }
            return Calculate(ExpressionHelper.GetExpressionPropertyName(field.Body), calculateOperator, value);
        }

        /// <summary>
        /// add value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Add(string name, dynamic value)
        {
            return Calculate(name, CalculateOperator.Add, value);
        }

        /// <summary>
        /// add value
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Add<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Add(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// subtract value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Subtract(string name, dynamic value)
        {
            return Calculate(name, CalculateOperator.subtract, value);
        }

        /// <summary>
        /// subtract value
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Subtract<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Subtract(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// multiply value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Multiply(string name, dynamic value)
        {
            return Calculate(name, CalculateOperator.multiply, value);
        }

        /// <summary>
        /// multiply value
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Multiply<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Multiply(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// divide value
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Divide(string name, dynamic value)
        {
            return Calculate(name, CalculateOperator.divide, value);
        }

        /// <summary>
        /// divide value
        /// </summary>
        /// <param name="field">field</param>
        /// <param name="value">value</param>
        /// <returns>IModify object</returns>
        public IModify Divide<T>(Expression<Func<T, dynamic>> field, dynamic value)
        {
            return Divide(ExpressionHelper.GetExpressionPropertyName(field.Body), value);
        }

        /// <summary>
        /// get modify values
        /// </summary>
        /// <returns>values</returns>
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
