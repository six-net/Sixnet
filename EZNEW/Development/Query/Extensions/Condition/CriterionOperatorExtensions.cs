using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    internal static class CriterionOperatorExtensions
    {
        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add(IQuery sourceQuery, CriterionOperator @operator, string fieldName, dynamic value, bool or = false, CriterionOptions criterionOptions = null)
        {
            return sourceQuery.AddCriterion(or ? ConditionConnectionOperator.OR : ConditionConnectionOperator.AND, fieldName, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add(IQuery sourceQuery, CriterionOperator @operator, string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                sourceQuery = sourceQuery.AddCriterion(or ? ConditionConnectionOperator.OR : ConditionConnectionOperator.AND, fieldName, @operator, subquery, new CriterionOptions()
                {
                    SubqueryValueFieldName = subqueryFieldName
                });
            }
            return sourceQuery;
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, CriterionOptions criterionOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddCriterion(or ? ConditionConnectionOperator.OR : ConditionConnectionOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, criterionOptions);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : IQueryModel<TQueryModel>
        {
            return or ? ConnectionExtensions.Or(sourceQuery, field, @operator, subquery) : ConnectionExtensions.And(sourceQuery, field, @operator, subquery);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TSourceQueryModel, TSubqueryQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : IQueryModel<TSourceQueryModel> where TSubqueryQueryModel : IQueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return sourceQuery;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return Add(sourceQuery, @operator, fieldName, subquery, subFieldName, or);
        }
    }
}
