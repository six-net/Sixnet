using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Develop.CQuery.CriteriaConverter;
using EZNEW.ExpressionUtil;

namespace EZNEW.Develop.CQuery
{
    public static class SortExtensions
    {
        #region ASC

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc(this IQuery sourceQuery, string field, ICriteriaConverter converter = null)
        {
            return sourceQuery.AddOrder(field, false, converter);
        }

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, ICriteriaConverter converter = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddOrder(ExpressionHelper.GetExpressionPropertyName(field.Body), false, converter);
        }

        #endregion

        #region DESC

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc(this IQuery sourceQuery, string field, ICriteriaConverter converter = null)
        {
            return sourceQuery.AddOrder(field, true, converter);
        }

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="converter">Field converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, ICriteriaConverter converter = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddOrder(ExpressionHelper.GetExpressionPropertyName(field.Body), true, converter);
        }

        #endregion 
    }
}
