using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using EZNEW.Exceptions;
using EZNEW.Expressions;

namespace EZNEW.Data.Modification
{
    /// <summary>
    /// Defines expression modification entry
    /// </summary>
    [Serializable]
    public class ExpressionModificationEntry : IModificationEntry
    {
        /// <summary>
        /// Gets the field name
        /// </summary>
        public string FieldName { get; private set; }

        /// <summary>
        /// Gets the modification value
        /// </summary>
        public IModificationValue Value { get; private set; }

        /// <summary>
        /// Initializes a new instance of ExpressionModificationEntry
        /// </summary>
        /// <param name="expression">Modification expression</param>
        public ExpressionModificationEntry(Expression expression)
        {
            ResolveExpression(expression);
        }

        /// <summary>
        /// Resolve expression
        /// </summary>
        void ResolveExpression(Expression expression)
        {
            if (expression is null)
            {
                throw new ArgumentNullException(nameof(expression));
            }
            if (expression.NodeType == ExpressionType.MemberAccess)
            {
                FieldName = ExpressionHelper.GetExpressionPropertyName(expression);
                Value = new FixedModificationValue(ExpressionHelper.GetExpressionValue(expression));
            }
            throw new EZNEWException($"Expression type:{expression?.GetType()} are not supported");
        }
    }
}
