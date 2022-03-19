using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class FullJoinExtensions
    {
        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="operator">Operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin(this IQuery sourceQuery, string sourceField, string targetField, CriterionOperator @operator, IQuery joinQuery)
        {
            return sourceQuery.Join(sourceField, targetField, JoinType.FullJoin, @operator, joinQuery);
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Operator</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin(this IQuery sourceQuery, CriterionOperator @operator, params IQuery[] joinQuerys)
        {
            if (!joinQuerys.IsNullOrEmpty())
            {
                foreach (var joinQuery in joinQuerys)
                {
                    sourceQuery = FullJoin(sourceQuery, string.Empty, string.Empty, @operator, joinQuery);
                }
            }
            return sourceQuery;
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="operator">Operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin(this IQuery sourceQuery, string joinField, CriterionOperator @operator, IQuery joinQuery)
        {
            return FullJoin(sourceQuery, joinField, joinField, @operator, joinQuery);
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="operator">Operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin<TSource, TTarget>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, CriterionOperator @operator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return FullJoin(sourceQuery, sourceFieldName, targetFieldName, @operator, joinQuery);
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="operator">Operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin<TSource>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> joinField, CriterionOperator @operator, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return FullJoin(sourceQuery, joinFieldName, joinFieldName, @operator, joinQuery);
        }
    }
}
