using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EZNEW.ExpressionUtil;

namespace EZNEW.Develop.Command.Modify
{
    /// <summary>
    /// Expression modify item
    /// </summary>
    [Serializable]
    public class ExpressionModifyItem : IModifyItem
    {
        bool isParsed = false;
        KeyValuePair<string, IModifyValue> parsedValue;

        /// <summary>
        /// Gets the modify expression
        /// </summary>
        public Expression Expression
        {
            get; private set;
        }

        /// <summary>
        /// Initializes a new instance of the EZNEW.Develop.Command.Modify.ExpressionModifyItem
        /// </summary>
        /// <param name="expression">Modify expression</param>
        public ExpressionModifyItem(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Parse modify value
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<string, IModifyValue> ParseModifyValue()
        {
            if (!isParsed)
            {
                string name = string.Empty;
                IModifyValue value = null;
                if (Expression != null && Expression.NodeType == ExpressionType.MemberAccess)
                {
                    name = ExpressionHelper.GetExpressionPropertyName(Expression);
                    value = new FixedModifyValue(ExpressionHelper.GetExpressionValue(Expression));
                }
                parsedValue = new KeyValuePair<string, IModifyValue>(name, value);
                isParsed = true;
            }
            return parsedValue;
        }
    }
}
