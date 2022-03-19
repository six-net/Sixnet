using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class NotEqualInnerJoinExtensions
    {
        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqualInnerJoin(this IQuery sourceQuery, string sourceField, string targetField, IQuery joinQuery)
        {
            return sourceQuery.InnerJoin(sourceField, targetField, CriterionOperator.NotEqual, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqualInnerJoin(this IQuery sourceQuery, string joinField, IQuery joinQuery)
        {
            return NotEqualInnerJoin(sourceQuery, joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TTarget">Join model type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <returns></returns>
        public static IQuery NotEqualInnerJoin<TTarget>(this IQuery sourceQuery) where TTarget : IQueryModel<TTarget>
        {
            return NotEqualInnerJoin<TTarget>(sourceQuery, null);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TTarget">Join model type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="targetFilter">Join model filter</param>
        /// <returns></returns>
        public static IQuery NotEqualInnerJoin<TTarget>(this IQuery sourceQuery, Expression<Func<TTarget, bool>> targetFilter) where TTarget : IQueryModel<TTarget>
        {
            return sourceQuery.Join(JoinType.InnerJoin, CriterionOperator.NotEqual, targetFilter);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqualInnerJoin<TSource, TTarget>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return NotEqualInnerJoin(sourceQuery, sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqualInnerJoin<TSource>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return NotEqualInnerJoin(sourceQuery, joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a inner join by using the NotEqual operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqualInnerJoin(this IQuery sourceQuery, params IQuery[] joinQuerys)
        {
            if (!joinQuerys.IsNullOrEmpty())
            {
                foreach (var query in joinQuerys)
                {
                    sourceQuery = NotEqualInnerJoin(sourceQuery, string.Empty, string.Empty, query);
                }
            }
            return sourceQuery;
        }
    }
}
