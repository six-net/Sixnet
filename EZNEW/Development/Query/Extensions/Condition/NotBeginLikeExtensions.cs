using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Development.Query.CriteriaConverter;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class NotBeginLikeExtensions
    {
        /// <summary>
        /// NotBeginLike Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotBeginLike(this IQuery sourceQuery, string fieldName, string value, bool or = false, ICriteriaConverter converter = null)
        {
            return sourceQuery.AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, fieldName, CriteriaOperator.NotBeginLike, value, converter);
        }

        /// <summary>
        /// NotBeginLike Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="or">Connect with 'and'(true/default) or 'or'(false)</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotBeginLike<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, string value, bool or = false, ICriteriaConverter converter = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddCriteria(or ? QueryOperator.OR : QueryOperator.AND, ExpressionHelper.GetExpressionPropertyName(field.Body), CriteriaOperator.NotBeginLike, value, converter);
        }
    }
}
