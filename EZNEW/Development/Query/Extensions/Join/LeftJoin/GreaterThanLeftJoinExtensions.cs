﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class GreaterThanLeftJoinExtensions
    {
        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery GreaterThanLeftJoin(this IQuery sourceQuery, string sourceField, string targetField, IQuery joinQuery)
        {
            return sourceQuery.LeftJoin(sourceField, targetField, CriterionOperator.GreaterThan, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery GreaterThanLeftJoin(this IQuery sourceQuery, string joinField, IQuery joinQuery)
        {
            return GreaterThanLeftJoin(sourceQuery, joinField, joinField, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TTarget">Join model type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <returns></returns>
        public static IQuery GreaterThanLeftJoin<TTarget>(this IQuery sourceQuery) where TTarget : IQueryModel<TTarget>
        {
            return GreaterThanLeftJoin<TTarget>(sourceQuery, null);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TTarget">Join model type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="targetFilter">Join model filter</param>
        /// <returns></returns>
        public static IQuery GreaterThanLeftJoin<TTarget>(this IQuery sourceQuery, Expression<Func<TTarget, bool>> targetFilter) where TTarget : IQueryModel<TTarget>
        {
            return sourceQuery.Join(JoinType.LeftJoin, CriterionOperator.GreaterThan, targetFilter);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <typeparam name="TTarget">Join target type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery GreaterThanLeftJoin<TSource, TTarget>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return GreaterThanLeftJoin(sourceQuery, sourceFieldName, targetFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <typeparam name="TSource">Join source type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinField">Join field</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery GreaterThanLeftJoin<TSource>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> joinField, IQuery joinQuery)
        {
            var joinFieldName = ExpressionHelper.GetExpressionPropertyName(joinField);
            return GreaterThanLeftJoin(sourceQuery, joinFieldName, joinFieldName, joinQuery);
        }

        /// <summary>
        /// Add a left join by using the GreaterThan operation
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinQuerys">Join querys</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery GreaterThanLeftJoin(this IQuery sourceQuery, params IQuery[] joinQuerys)
        {
            if (!joinQuerys.IsNullOrEmpty())
            {
                foreach (var query in joinQuerys)
                {
                    sourceQuery = GreaterThanLeftJoin(sourceQuery, string.Empty, string.Empty, query);
                }
            }
            return sourceQuery;
        }
    }
}
