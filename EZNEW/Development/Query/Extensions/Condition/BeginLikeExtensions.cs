using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class BeginLikeExtensions
    {
        /// <summary>
        /// BeginLike condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery BeginLike(this IQuery sourceQuery, FieldInfo field, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.AddCriterion(connector, field, CriterionOperator.BeginLike, value, criterionOptions);
        }

        /// <summary>
        /// BeginLike condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery BeginLike(this IQuery sourceQuery, string fieldName, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.BeginLike(FieldInfo.Create(fieldName), value, criterionOptions, connector);
        }

        /// <summary>
        /// BeginLike condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery BeginLike<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            string fieldName = ExpressionHelper.GetExpressionPropertyName(field.Body);
            return sourceQuery.BeginLike(fieldName, value, criterionOptions, connector);
        }
    }
}
