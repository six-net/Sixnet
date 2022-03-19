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
        /// <param name="field">Field</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, FieldInfo field, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null)
        {
            return Connect(sourceQuery, CriterionConnector.And, field, @operator, value, criterionOptions);
        }

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
            return Connect(sourceQuery, CriterionConnector.And, fieldName, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="groupFieldConnector">Group field connector</param>
        /// <param name="groupFieldNames">Field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, CriterionConnector groupFieldConnector, CriterionOperator @operator, dynamic value, params string[] groupFieldNames)
        {
            return And(sourceQuery, @operator, value, null, groupFieldConnector, groupFieldNames);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="groupFieldConnector">Group field connection operator</param>
        /// <param name="groupFieldNames">Group field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, CriterionConnector groupFieldConnector, params string[] groupFieldNames)
        {
            return Connect(sourceQuery, CriterionConnector.And, @operator, value, criterionOptions, groupFieldConnector, groupFieldNames);
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
            return Connect(sourceQuery, CriterionConnector.And, conditionExpression);
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
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="groupFieldConnector">Group field connector</param>
        /// <param name="groupFields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, CriterionOperator @operator, dynamic value, CriterionConnector groupFieldConnector, params Expression<Func<TQueryModel, dynamic>>[] groupFields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return And<TQueryModel>(sourceQuery, @operator, value, null, groupFieldConnector, groupFields);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criteria converter</param>
        /// <param name="groupFieldConnector">Group field connector</param>
        /// <param name="groupFields">Fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And<TQueryModel>(this IQuery sourceQuery, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, CriterionConnector groupFieldConnector, params Expression<Func<TQueryModel, dynamic>>[] groupFields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, CriterionConnector.And, @operator, value, criterionOptions, groupFieldConnector, groupFields);
        }

        /// <summary>
        /// Connect condition with 'and'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery And(this IQuery sourceQuery, IQuery groupQuery)
        {
            return Connect(sourceQuery, CriterionConnector.And, groupQuery);
        }

        #endregion

        #region Or

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, FieldInfo field, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null)
        {
            return Connect(sourceQuery, CriterionConnector.Or, field, @operator, value, criterionOptions);
        }

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
            return Connect(sourceQuery, CriterionConnector.Or, fieldName, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="groupFieldConnector">Group field connector</param>
        /// <param name="groupFieldNames">Group field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, CriterionOperator @operator, dynamic value, CriterionConnector groupFieldConnector, params string[] groupFieldNames)
        {
            return Or(sourceQuery, @operator, value, null, groupFieldConnector, groupFieldNames);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="groupFieldConnector">Group field connector</param>
        /// <param name="groupFieldNames">Group field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, CriterionConnector groupFieldConnector, params string[] groupFieldNames)
        {
            return Connect(sourceQuery, CriterionConnector.Or, @operator, value, criterionOptions, groupFieldConnector, groupFieldNames);
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
            return Connect(sourceQuery, CriterionConnector.Or, conditionExpression);
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
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="groupFieldConnector">Group field connector</param>
        /// <param name="groupFields">Group fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or<TQueryModel>(this IQuery sourceQuery, CriterionOperator @operator, dynamic value, CriterionConnector groupFieldConnector, params Expression<Func<TQueryModel, dynamic>>[] groupFields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Or<TQueryModel>(sourceQuery, @operator, value, null, groupFieldConnector, groupFields);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="groupFieldConnector">Groupo field connector</param>
        /// <param name="groupFields">Group fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or<TQueryModel>(this IQuery sourceQuery, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, CriterionConnector groupFieldConnector, params Expression<Func<TQueryModel, dynamic>>[] groupFields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, CriterionConnector.Or, @operator, value, criterionOptions, groupFieldConnector, groupFields);
        }

        /// <summary>
        /// Connect condition with 'or'
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Or(this IQuery sourceQuery, IQuery groupQuery)
        {
            return Connect(sourceQuery, CriterionConnector.Or, groupQuery);
        }

        #endregion

        #region Connect

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connector">Connector</param>
        /// <param name="field">Field</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect(IQuery sourceQuery, CriterionConnector connector, FieldInfo field, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null)
        {
            return sourceQuery.AddCriterion(connector, field, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connector">Connector</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect(IQuery sourceQuery, CriterionConnector connector, string fieldName, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null)
        {
            return Connect(sourceQuery, connector, FieldInfo.Create(fieldName), @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connector">Connector</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="groupFieldConnector">Each field connector</param>
        /// <param name="groupFieldNames">Group field names</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect(IQuery sourceQuery, CriterionConnector connector, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, CriterionConnector groupFieldConnector, params string[] groupFieldNames)
        {
            if (groupFieldNames.IsNullOrEmpty())
            {
                return sourceQuery;
            }
            IQuery groupQuery = QueryManager.Create();
            foreach (string field in groupFieldNames)
            {
                groupQuery = Connect(groupQuery, groupFieldConnector, field, @operator, value, criterionOptions);
            }
            groupQuery.Connector = connector;
            return sourceQuery.AddCondition(groupQuery);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connector">Connector</param>
        /// <param name="conditionExpression">Condition expression</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnector connector, Expression<Func<TQueryModel, bool>> conditionExpression) where TQueryModel : IQueryModel<TQueryModel>
        {
            var expressionCondition = ExpressionQueryHelper.GetExpressionCondition(connector, conditionExpression.Body);
            if (expressionCondition != null)
            {
                sourceQuery = sourceQuery.AddCondition(expressionCondition);
            }
            return sourceQuery;
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connector">Connector</param>
        /// <param name="field">Field</param>
        /// <param name="operator">criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnector connector, Expression<Func<TQueryModel, dynamic>> field, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions = null) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect(sourceQuery, connector, ExpressionHelper.GetExpressionPropertyName(field.Body), @operator, value, criterionOptions);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connector">Connector</param>
        /// <param name="operator">Condition operator</param>
        /// <param name="value">Value</param>
        /// <param name="groupFieldConnector">Group field connection operator</param>
        /// <param name="groupFields">Group fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnector connector, CriterionOperator @operator, dynamic value, CriterionConnector groupFieldConnector, params Expression<Func<TQueryModel, dynamic>>[] groupFields) where TQueryModel : IQueryModel<TQueryModel>
        {
            return Connect<TQueryModel>(sourceQuery, connector, @operator, value, null, groupFieldConnector, groupFields);
        }

        /// <summary>
        /// Connect condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connector">Connector</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criteria converter</param>
        /// <param name="groupFieldConnector">Group field connection operator</param>
        /// <param name="groupFields">Group fields</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect<TQueryModel>(IQuery sourceQuery, CriterionConnector connector, CriterionOperator @operator, dynamic value, CriterionOptions criterionOptions, CriterionConnector groupFieldConnector, params Expression<Func<TQueryModel, dynamic>>[] groupFields) where TQueryModel : IQueryModel<TQueryModel>
        {
            if (groupFields.IsNullOrEmpty())
            {
                return sourceQuery;
            }
            IEnumerable<string> fieldNames = groupFields.Select(c => ExpressionHelper.GetExpressionPropertyName(c.Body));
            return Connect(sourceQuery, connector, @operator, value, criterionOptions, groupFieldConnector, fieldNames.ToArray());
        }

        /// <summary>
        /// Connect group query
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="connectionOperator">Connection operator</param>
        /// <param name="groupQuery">Group query condition</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Connect(IQuery sourceQuery, CriterionConnector connectionOperator, IQuery groupQuery)
        {
            groupQuery.Connector = connectionOperator;
            return sourceQuery.AddCondition(groupQuery);
        }

        #endregion
    }
}
