using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Development.Query.CriteriaConverter;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class AndExtensions
    {
        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, string fieldName, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null)
        {
            return sourceQuery.AddCriteria(QueryOperator.AND, fieldName, @operator, value, converter);
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectOperator">each field codition connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, params string[] fieldNames)
        {
            return And(sourceQuery, eachFieldConnectOperator, @operator, value, null, fieldNames);
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectOperator">each field codition connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter, params string[] fieldNames)
        {
            if (fieldNames.IsNullOrEmpty())
            {
                return sourceQuery;
            }
            IQuery groupQuery = QueryManager.Create();
            foreach (string field in fieldNames)
            {
                switch (eachFieldConnectOperator)
                {
                    case QueryOperator.AND:
                    default:
                        groupQuery = And(groupQuery, field, @operator, value, converter);
                        break;
                    case QueryOperator.OR:
                        groupQuery = OrExtensions.Or(groupQuery, field, @operator, value, converter);
                        break;
                }
            }
            return sourceQuery.AddQueryItem(QueryOperator.AND, groupQuery);
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="criteria">Criteria</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, bool>> criteria) where TQueryModel : IQueryModel<TQueryModel>
        {
            var expressQuery = ExpressionQueryHelper.GetExpressionQuery(QueryOperator.AND, criteria.Body);
            if (expressQuery != null)
            {
                sourceQuery = sourceQuery.AddQueryItem(expressQuery.Item1, expressQuery.Item2);
            }
            return sourceQuery;
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return And(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, converter);
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return And<TQueryModel>(sourceQuery, eachFieldConnectOperator, @operator, value, null, fields);
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectOperator">Each field connect operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="converter">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, QueryOperator eachFieldConnectOperator, CriteriaOperator @operator, dynamic value, ICriteriaConverter converter, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            if (fields.IsNullOrEmpty())
            {
                return sourceQuery;
            }
            IEnumerable<string> fieldNames = fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body));
            return And(sourceQuery, eachFieldConnectOperator, @operator, value, converter, fieldNames.ToArray());
        }

        /// <summary>
        /// Add a condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, IQuery groupQuery)
        {
            return sourceQuery.AddQueryItem(QueryOperator.AND, groupQuery);
        }
    }
}
