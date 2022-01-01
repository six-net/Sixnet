using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class LessThanOrEqualExtensions
    {
        /// <summary>
        /// LessThanOrEqual condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqual(this IQuery sourceQuery, string fieldName, dynamic value, bool or = false, CriterionOptions criterionOptions = null)
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.LessThanOrEqual, fieldName, value, or, criterionOptions);
        }

        /// <summary>
        /// LessThanOrEqual condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqual(this IQuery sourceQuery, string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.LessThanOrEqual, fieldName, subquery, subqueryFieldName, or);
        }

        /// <summary>
        /// LessThanOrEqual condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, CriterionOptions criterionOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.LessThanOrEqual, field, value, or, criterionOptions);
        }

        /// <summary>
        /// LessThanOrEqual condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : IQueryModel<TQueryModel>
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.LessThanOrEqual, field, subquery, or);
        }

        /// <summary>
        /// LessThanOrEqual condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery LessThanOrEqual<TSourceQueryModel, TSubqueryQueryModel>(this IQuery sourceQuery, Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : IQueryModel<TSourceQueryModel> where TSubqueryQueryModel : IQueryModel<TSubqueryQueryModel>
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.LessThanOrEqual, field, subquery, subqueryField, or);
        }
    }
}
