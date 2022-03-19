using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class EndLikeExtensions
    {
        /// <summary>
        /// EndLike Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery EndLike(this IQuery sourceQuery, FieldInfo field, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.AddCriterion(connector, field, CriterionOperator.EndLike, value, criterionOptions);
        }

        /// <summary>
        /// EndLike Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery EndLike(this IQuery sourceQuery, string fieldName, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return EndLike(sourceQuery, FieldInfo.Create(fieldName), value, criterionOptions, connector);
        }

        /// <summary>
        /// EndLike Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery EndLike<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            string fieldName = ExpressionHelper.GetExpressionPropertyName(field.Body);
            return EndLike(sourceQuery, fieldName, value, criterionOptions, connector);
        }
    }
}
