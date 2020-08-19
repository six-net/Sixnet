using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.ExpressionUtil;

namespace EZNEW.Develop.CQuery
{
    public static class FullJoinExtensions
    {
        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin(this IQuery sourceQuery, string sourceField, string targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return sourceQuery.Join(sourceField, targetField, JoinType.FullJoin, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin(this IQuery sourceQuery, JoinOperator joinOperator, params IQuery[] joinQuerys)
        {
            if (!joinQuerys.IsNullOrEmpty())
            {
                foreach (var joinQuery in joinQuerys)
                {
                    sourceQuery = FullJoin(sourceQuery, string.Empty, string.Empty, joinOperator, joinQuery);
                }
            }
            return sourceQuery;
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin(this IQuery sourceQuery, string joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            return FullJoin(sourceQuery, joinField, joinField, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin<TSource, TTarget>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return FullJoin(sourceQuery, sourceFieldName, targetFieldName, joinOperator, joinQuery);
        }

        /// <summary>
        /// Add a full join query
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery FullJoin<TSource>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> joinField, JoinOperator joinOperator, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return FullJoin(sourceQuery, joinFieldName, joinFieldName, joinOperator, joinQuery);
        }
    }
}
