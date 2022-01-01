using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class EndLikeExtensions
    {
        /// <summary>
        /// EndLike Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery EndLike(this IQuery sourceQuery, string fieldName, string value, bool or = false, CriterionOptions criterionOptions = null)
        {
            return sourceQuery.AddCriterion(or ? ConditionConnectionOperator.OR : ConditionConnectionOperator.AND, fieldName, CriterionOperator.EndLike, value, criterionOptions);
        }

        /// <summary>
        /// EndLike Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery EndLike<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, CriterionOptions criterionOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddCriterion(or ? ConditionConnectionOperator.OR : ConditionConnectionOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriterionOperator.EndLike, value, criterionOptions);
        }
    }
}
