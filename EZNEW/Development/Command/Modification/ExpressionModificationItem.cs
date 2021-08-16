using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EZNEW.Expressions;

namespace EZNEW.Development.Command.Modification
{
    /// <summary>
    /// Defines expression modification item
    /// </summary>
    [Serializable]
    public class ExpressionModificationItem : IModificationItem
    {
        bool isParsed = false;
        KeyValuePair<string, IModificationValue> parsedValue;

        /// <summary>
        /// Gets the modification expression
        /// </summary>
        public Expression Expression { get; private set; }

        /// <summary>
        /// Initializes a new instance of the EZNEW.Development.Command.Modification.ExpressionModificationItem
        /// </summary>
        /// <param name="expression">Modification expression</param>
        public ExpressionModificationItem(Expression expression)
        {
            Expression = expression;
        }

        /// <summary>
        /// Resolve value
        /// </summary>
        /// <returns></returns>
        public KeyValuePair<string, IModificationValue> ResolveValue()
        {
            if (!isParsed)
            {
                string name = string.Empty;
                IModificationValue value = null;
                if (Expression != null && Expression.NodeType == ExpressionType.MemberAccess)
                {
                    name = ExpressionHelper.GetExpressionPropertyName(Expression);
                    value = new FixedModificationValue(ExpressionHelper.GetExpressionValue(Expression));
                }
                parsedValue = new KeyValuePair<string, IModificationValue>(name, value);
                isParsed = true;
            }
            return parsedValue;
        }
    }
}
