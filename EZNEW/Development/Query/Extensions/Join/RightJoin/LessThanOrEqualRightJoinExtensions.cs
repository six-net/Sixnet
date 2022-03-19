using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class LessThanOrEqualRightJoinExtensions
    {
        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqualRightJoin(this IQuery sourceQuery, string sourceField, string targetField, IQuery joinQuery)
        {
            return sourceQuery.RightJoin(sourceField, targetField, CriterionOperator.LessThanOrEqual, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqualRightJoin(this IQuery sourceQuery, string joinField, IQuery joinQuery)
        {
            return LessThanOrEqualRightJoin(sourceQuery, joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TTarget">Join model type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <returns></returns>
        public static IQuery LessThanOrEqualRightJoin<TTarget>(this IQuery sourceQuery) where TTarget : IQueryModel<TTarget>
        {
            return LessThanOrEqualRightJoin<TTarget>(sourceQuery, null);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TTarget">Join model type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="targetFilter">Join model filter</param>
        /// <returns></returns>
        public static IQuery LessThanOrEqualRightJoin<TTarget>(this IQuery sourceQuery, Expression<Func<TTarget, bool>> targetFilter) where TTarget : IQueryModel<TTarget>
        {
            return sourceQuery.Join(JoinType.RightJoin, CriterionOperator.LessThanOrEqual, targetFilter);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqualRightJoin<TSource, TTarget>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return LessThanOrEqualRightJoin(sourceQuery, sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqualRightJoin<TSource>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return LessThanOrEqualRightJoin(sourceQuery, joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a right join by using the LessThanOrEqual operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqualRightJoin(this IQuery sourceQuery, params IQuery[] joinQuerys)
        {
            if (!joinQuerys.IsNullOrEmpty())
            {
                foreach (var query in joinQuerys)
                {
                    sourceQuery = LessThanOrEqualRightJoin(sourceQuery, string.Empty, string.Empty, query);
                }
            }
            return sourceQuery;
        }
    }
}
