using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class SortExtensions
    {
        #region ASC

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc(this IQuery sourceQuery, string field, SortOptions sortOptions = null)
        {
            return sourceQuery.AddSort(field, false, sortOptions);
        }

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, SortOptions sortOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddSort(ExpressionHelper.GetExpressionPropertyName(field.Body), false, sortOptions);
        }

        #endregion

        #region DESC

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc(this IQuery sourceQuery, string field, SortOptions sortOptions = null)
        {
            return sourceQuery.AddSort(field, true, sortOptions);
        }

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, SortOptions sortOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddSort(ExpressionHelper.GetExpressionPropertyName(field.Body), true, sortOptions);
        }

        #endregion 
    }
}
