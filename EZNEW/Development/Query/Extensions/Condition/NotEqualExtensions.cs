﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Development.Query.CriteriaConverter;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class NotEqualExtensions
    {
        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual(this IQuery sourceQuery, string fieldName, dynamic value, bool or = false, ICriteriaConverter converter = null)
        {
            return sourceQuery.AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotEqual, value, converter);
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual(this IQuery sourceQuery, string fieldName, IQuery subquery, string subqueryFieldName = "", bool or = false)
        {
            if (subquery != null)
            {
                sourceQuery = sourceQuery.AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotEqual, subquery, null, new QueryParameterOptions()
                {
                    QueryFieldName = subqueryFieldName
                });
            }
            return sourceQuery;
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, dynamic value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotEqual, value, converter);
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, bool or = false) where TQueryModel : IQueryModel<TQueryModel>
        {
            return or ? OrExtensions.Or(sourceQuery, field, CriteriaOperator.NotEqual, subquery) : AndExtensions.And(sourceQuery, field, CriteriaOperator.NotEqual, subquery);
        }

        /// <summary>
        /// Not Equal Condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TSourceQueryModel, TSubqueryQueryModel>(this IQuery sourceQuery, Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, bool or = false) where TSourceQueryModel : IQueryModel<TSourceQueryModel> where TSubqueryQueryModel : IQueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return sourceQuery;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return NotEqual(sourceQuery, fieldName, subquery, subFieldName, or);
        }
    }
}
