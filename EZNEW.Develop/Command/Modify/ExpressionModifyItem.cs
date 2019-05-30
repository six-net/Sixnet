using EZNEW.Framework.ExpressionUtil;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace EZNEW.Develop.Command.Modify
{
    public class ExpressionModifyItem : IModifyItem
    {
        bool isParsed = false;
        KeyValuePair<string, IModifyValue> parsedValue;

        /// <summary>
        /// expression
        /// </summary>
        public Expression Expression
        {
            get; private set;
        }

        public ExpressionModifyItem(Expression expression)
        {
            Expression = expression;
        }

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
