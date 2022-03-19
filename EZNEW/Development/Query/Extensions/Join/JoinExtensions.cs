using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Exceptions;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class JoinExtensions
    {
        /// <summary>
        /// Add join
        /// </summary>
        /// <typeparam name="TTarget">Target type</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinType">Join type</param>
        /// <param name="operator">Operator</param>
        /// <param name="targetFilter"Target filter</param>
        /// <returns></returns>
        public static IQuery Join<TTarget>(this IQuery sourceQuery, JoinType joinType, CriterionOperator @operator, Expression<Func<TTarget, bool>> targetFilter = null) where TTarget : IQueryModel<TTarget>
        {
            IQuery joinObjectFilter = QueryManager.Create<TTarget>();
            if (targetFilter != null)
            {
                joinObjectFilter.And(targetFilter);
            }
            return Join(sourceQuery, new Dictionary<string, string>(0), joinType, @operator, joinObjectFilter);
        }

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinFields">Join fields(Key:source table field,Value: target table field)</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinOperator">Join operator</param>
        /// <param name="joinObjectFilter">Join object filter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Join(this IQuery sourceQuery, Dictionary<string, string> joinFields, JoinType joinType, CriterionOperator @operator, IQuery joinObjectFilter)
        {
            joinFields = joinFields?.Where(c => !string.IsNullOrWhiteSpace(c.Key) && !string.IsNullOrWhiteSpace(c.Value)).ToDictionary(c => c.Key, c => c.Value);
            return Join(sourceQuery, joinFields?.Select(jf =>
             {
                 return RegularJoinCriterion.Create(FieldInfo.Create(jf.Key), @operator, FieldInfo.Create(jf.Value)) as IJoinCriterion;
             }), joinType, joinObjectFilter);
        }

        /// <summary>
        /// Add join
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinCriteria">Join criteria</param>
        /// <param name="joinType">Join type</param>
        /// <param name="joinObjectFilter">Join object filter</param>
        /// <returns></returns>
        public static IQuery Join(this IQuery sourceQuery, IEnumerable<IJoinCriterion> joinCriteria, JoinType joinType, IQuery joinObjectFilter)
        {
            if (joinObjectFilter?.GetEntityType() == null)
            {
                throw new EZNEWException("The IQuery object used for join must set EntityType.");
            }
            return sourceQuery.Join(new JoinEntry()
            {
                Type = joinType,
                JoinObjectFilter = joinObjectFilter,
                JoinCriteria = joinCriteria?.ToList(),
            });
        }

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="sourceField">Source field</param>
        /// <param name="targetField">Target field</param>
        /// <param name="joinType">Join type</param>
        /// <param name="operator">Operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Join(this IQuery sourceQuery, string sourceField, string targetField, JoinType joinType, CriterionOperator @operator, IQuery joinQuery)
        {
            return sourceQuery.Join(new Dictionary<string, string>(1)
            {
                 { sourceField,targetField }
            }, joinType, @operator, joinQuery);
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
        /// <param name="operator">Operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Join<TSource, TTarget>(this IQuery sourceQuery, Expression<Func<TSource, dynamic>> sourceField, Expression<Func<TTarget, dynamic>> targetField, JoinType joinType, CriterionOperator @operator, IQuery joinQuery)
        {
            var sourceFieldName = ExpressionHelper.GetExpressionPropertyName(sourceField);
            var targetFieldName = ExpressionHelper.GetExpressionPropertyName(targetField);
            return Join(sourceQuery, sourceFieldName, targetFieldName, joinType, @operator, joinQuery);
        }

        /// <summary>
        /// Add a join query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="joinType">Join type</param>
        /// <param name="operator">Operator</param>
        /// <param name="joinQuery">Join query</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Join(this IQuery sourceQuery, JoinType joinType, CriterionOperator @operator, IQuery joinQuery)
        {
            return Join(sourceQuery, string.Empty, string.Empty, joinType, @operator, joinQuery);
        }
    }
}
