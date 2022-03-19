using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Data.Conversion;
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
        public static IQuery Asc(this IQuery sourceQuery, FieldInfo field, SortOptions sortOptions = null)
        {
            return sourceQuery.AddSort(new SortEntry()
            {
                Desc = false,
                Field = field,
                Options = sortOptions
            });
        }

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc(this IQuery sourceQuery, string fieldName, SortOptions sortOptions = null)
        {
            return Asc(sourceQuery, FieldInfo.Create(fieldName), sortOptions);
        }

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Asc(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body));
        }

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, SortOptions sortOptions) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Asc(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body), sortOptions);
        }

        /// <summary>
        /// Order by asc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="fieldConversionOptions">Field conversion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Asc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, FieldConversionOptions fieldConversionOptions) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Asc(sourceQuery, FieldInfo.Create(ExpressionHelper.GetExpressionPropertyName(field.Body), fieldConversionOptions));
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
        public static IQuery Desc(this IQuery sourceQuery, FieldInfo field, SortOptions sortOptions = null)
        {
            return sourceQuery.AddSort(new SortEntry()
            {
                Desc = true,
                Field = field,
                Options = sortOptions
            });
        }

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc(this IQuery sourceQuery, string field, SortOptions sortOptions = null)
        {
            return Desc(sourceQuery, FieldInfo.Create(field), sortOptions);
        }

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Desc(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body));
        }

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="sortOptions">Sort options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, SortOptions sortOptions) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Desc(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body), sortOptions);
        }

        /// <summary>
        /// Order by desc
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="fieldConversionOptions">Field conversion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Desc<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, FieldConversionOptions fieldConversionOptions) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Desc(sourceQuery, FieldInfo.Create(ExpressionHelper.GetExpressionPropertyName(field.Body), fieldConversionOptions));
        }

        #endregion 
    }
}
