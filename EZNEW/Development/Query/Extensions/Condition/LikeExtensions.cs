using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class LikeExtensions
    {

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Like(this IQuery sourceQuery, FieldInfo field, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.AddCriterion(connector, field, CriterionOperator.Like, value, criterionOptions);
        }

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Like(this IQuery sourceQuery, string fieldName, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And)
        {
            return Like(sourceQuery, FieldInfo.Create(fieldName), value, criterionOptions, connector);
        }

        /// <summary>
        /// Like Condition
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="value">Value</param>
        /// <param name="criterionOptions">Criterion options</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery Like<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, string value, CriterionOptions criterionOptions = null, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            string fieldName = ExpressionHelper.GetExpressionPropertyName(field.Body);
            return Like(sourceQuery, fieldName, value, criterionOptions, connector);
        }
    }
}
