using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Data.Conversion;
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
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add(IQuery sourceQuery, CriterionOperator @operator, FieldInfo field, dynamic value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.AddCriterion(connector, field, @operator, value, criterionOptions);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add(IQuery sourceQuery, CriterionOperator @operator, string fieldName, dynamic value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.AddCriterion(connector, FieldInfo.Create(fieldName), @operator, value, criterionOptions);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add(IQuery sourceQuery, CriterionOperator @operator, FieldInfo field, IQuery subquery, string subqueryFieldName = "", CriterionConnector connector = CriterionConnector.And)
        {
            if (subquery != null)
            {
                sourceQuery = sourceQuery.AddCriterion(connector, field, @operator, subquery, new CriterionOptions()
                {
                    SubqueryField = subqueryFieldName
                });
            }
            return sourceQuery;
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add(IQuery sourceQuery, CriterionOperator @operator, string fieldName, IQuery subquery, string subqueryFieldName = "", CriterionConnector connector = CriterionConnector.And)
        {
            return Add(sourceQuery, @operator, FieldInfo.Create(fieldName), subquery, subqueryFieldName, connector);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TQueryModel, dynamic>> field, dynamic value, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            string fieldName = ExpressionHelper.GetExpressionPropertyName(field.Body);
            return Add(sourceQuery, @operator, fieldName, value,  null, connector);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TQueryModel, dynamic>> field, dynamic value, CriterionOptions criterionOptions, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            string fieldName = ExpressionHelper.GetExpressionPropertyName(field.Body);
            return Add(sourceQuery, @operator, fieldName, value, criterionOptions, connector);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="fieldConversionOptions">Field conversion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TQueryModel, dynamic>> field, dynamic value, FieldConversionOptions fieldConversionOptions, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            string fieldName = ExpressionHelper.GetExpressionPropertyName(field.Body);
            return Add(sourceQuery, @operator, FieldInfo.Create(fieldName, fieldConversionOptions), value, null, connector);
        }

        /// <summary>
        /// Add condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="operator">Criterion operator</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            return connector == CriterionConnector.Or ? ConnectionExtensions.Or(sourceQuery, field, @operator, subquery) : ConnectionExtensions.And(sourceQuery, field, @operator, subquery);
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
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        internal static IQuery Add<TSourceQueryModel, TSubqueryQueryModel>(IQuery sourceQuery, CriterionOperator @operator, Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, CriterionConnector connector = CriterionConnector.And) where TSourceQueryModel : IQueryModel<TSourceQueryModel> where TSubqueryQueryModel : IQueryModel<TSubqueryQueryModel>
        {
            if (field == null || subquery == null || subqueryField == null)
            {
                return sourceQuery;
            }
            var fieldName = ExpressionHelper.GetExpressionPropertyName(field);
            var subFieldName = ExpressionHelper.GetExpressionPropertyName(subqueryField);
            return Add(sourceQuery, @operator, fieldName, subquery, subFieldName, connector);
        }
    }
}
