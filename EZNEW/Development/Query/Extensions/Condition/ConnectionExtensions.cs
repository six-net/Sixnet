using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class ConnectionExtensions
    {
        #region And

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, string fieldName, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null)
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, fieldName, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, params string[] fieldNames)
        {
            return And(sourceQuery, eachFieldConnectionOperator, @operator, value, null, fieldNames);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, params string[] fieldNames)
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, eachFieldConnectionOperator, @operator, value, criterionOptions, fieldNames);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, bool>> conditionExpression) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, conditionExpression);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="operator">criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return And(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return And<TQueryModel>(sourceQuery, eachFieldConnectionOperator, @operator, value, null, fields);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, eachFieldConnectionOperator, @operator, value, criterionOptions, fields);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, IQuery groupQuery)
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, groupQuery);
        }

        #endregion

        #region Or

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, string fieldName, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null)
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, fieldName, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, params string[] fieldNames)
        {
            return Or(sourceQuery, eachFieldConnectionOperator, @operator, value, null, fieldNames);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, params string[] fieldNames)
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, eachFieldConnectionOperator, @operator, value, criterionOptions, fieldNames);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, bool>> conditionExpression) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, conditionExpression);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="operator">criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Or(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or<TQueryModel>(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Or<TQueryModel>(sourceQuery, eachFieldConnectionOperator, @operator, value, null, fields);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or<TQueryModel>(this IQuery sourceQuery, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, eachFieldConnectionOperator, @operator, value, criterionOptions, fields);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, IQuery groupQuery)
        {
            return Connect(sourceQuery, CriterionConnectionOperator.Or, groupQuery);
        }

        #endregion

        #region Connect

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect(IQuery sourceQuery, CriterionConnectionOperator connectionOperator, string fieldName, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null)
        {
            return sourceQuery.AddCriterion(connectionOperator, fieldName, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="fieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect(IQuery sourceQuery, CriterionConnectionOperator connectionOperator, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, params string[] fieldNames)
        {
            if (fieldNames.IsNullOrEmpty())
            {
                return sourceQuery;
            }
            IQuery groupQuery = QueryManager.Create();
            foreach (string field in fieldNames)
            {
                groupQuery = Connect(groupQuery, eachFieldConnectionOperator, field, @operator, value, criterionOptions);
            }
            groupQuery.ConnectionOperator = connectionOperator;
            return sourceQuery.AddCondition(groupQuery);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnectionOperator connectionOperator, Expression<Func<TQueryModel, bool>> conditionExpression) where TQueryModel : IQueryModel<TQueryModel>
        {
            var expressCondition = ExpressionQueryHelper.GetExpressionCondition(connectionOperator, conditionExpression.Body);
            if (expressCondition != null)
            {
                sourceQuery = sourceQuery.AddCondition(expressCondition);
            }
            return sourceQuery;
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="field">Field</param>
        /// <param name="operator">criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnectionOperator connectionOperator, Expression<Func<TQueryModel, dynamic>> field, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, connectionOperator, ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnectionOperator connectionOperator, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect<TQueryModel>(sourceQuery, connectionOperator, eachFieldConnectionOperator, @operator, value, null, fields);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="eachFieldConnectionOperator">Each field connection operator</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criteria converter</param>
        /// <param name="fields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnectionOperator connectionOperator, CriterionConnectionOperator eachFieldConnectionOperator, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, params Expression<Func<TQueryModel, dynamic>>[] fields) where TQueryModel : IQueryModel<TQueryModel>
        {
            if (fields.IsNullOrEmpty())
            {
                return sourceQuery;
            }
            IEnumerable<string> fieldNames = fields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body));
            return Connect(sourceQuery, connectionOperator, eachFieldConnectionOperator, @operator, value, criterionOptions, fieldNames.ToArray());
        }

        /// <summary>
        /// Connect group query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect(IQuery sourceQuery, CriterionConnectionOperator connectionOperator, IQuery groupQuery)
        {
            groupQuery.ConnectionOperator = connectionOperator;
            return sourceQuery.AddCondition(groupQuery);
        }

        #endregion
    }
}
