using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class JoinExtensions
    {

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Join(this IQuery sourceQuery, string sourceField, string targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery)
        {
            return sourceQuery.Join(new Dictionary<string, string>(1)
                   {
                        { sourceField,targetField }
                   }, joinType, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Join<TSource, TTarget>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return Join(sourceQuery, sourceFieldName, targetFieldName, joinType, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Join(this IQuery sourceQuery, JoinType joinType, JoinOperator joinOperator, IQuery joinQuery)
        {
            return Join(sourceQuery, string.Empty, string.Empty, joinType, joinOperator, joinQuery);
        }
    }
}
