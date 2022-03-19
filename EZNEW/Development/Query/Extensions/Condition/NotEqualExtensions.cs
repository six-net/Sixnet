using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Data.Conversion;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class NotEqualExtensions
    {
        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual(this IQuery sourceQuery, FieldInfo field, dynamic value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.NotEqual, field, value, criterionOptions, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual(this IQuery sourceQuery, string fieldName, dynamic value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return NotEqual(sourceQuery, FieldInfo.Create(fieldName), value, criterionOptions, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual(this IQuery sourceQuery, FieldInfo field, IQuery subquery, string subqueryFieldName = "", CriterionConnector connector = CriterionConnector.And)
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.NotEqual, field, subquery, subqueryFieldName, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryFieldName">Subquery field</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual(this IQuery sourceQuery, string fieldName, IQuery subquery, string subqueryFieldName = "", CriterionConnector connector = CriterionConnector.And)
        {
            return NotEqual(sourceQuery, FieldInfo.Create(fieldName), subquery, subqueryFieldName, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, dynamic value, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            CriterionOptions criterionOptions = null;
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.NotEqual, field, value, criterionOptions, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, dynamic value, CriterionOptions criterionOptions, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.NotEqual, field, value, criterionOptions, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="fieldConversionOptions">Field conversion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, dynamic value, FieldConversionOptions fieldConversionOptions, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.NotEqual, field, value, fieldConversionOptions, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, IQuery subquery, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.NotEqual, field, subquery, connector);
        }

        /// <summary>
        /// NotEqual condition
        /// </summary>
        /// <typeparam name="TSourceQueryModel">Source query model</typeparam>
        /// <typeparam name="TSubqueryQueryModel">Subquery query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="subquery">Subquery</param>
        /// <param name="subqueryField">Subquery field</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotEqual<TSourceQueryModel, TSubqueryQueryModel>(this IQuery sourceQuery, Expression<Func<TSourceQueryModel, dynamic>> field, IQuery subquery, Expression<Func<TSubqueryQueryModel, dynamic>> subqueryField, CriterionConnector connector = CriterionConnector.And) where TSourceQueryModel : IQueryModel<TSourceQueryModel> where TSubqueryQueryModel : IQueryModel<TSubqueryQueryModel>
        {
            return CriterionOperatorExtensions.Add(sourceQuery, CriterionOperator.NotEqual, field, subquery, subqueryField, connector);
        }
    }
}
