using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class IsNullExtensions
    {
        /// <summary>
        /// Field is null
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery IsNull(this IQuery sourceQuery, string fieldName, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.AddCriterion(connector, FieldInfo.Create(fieldName), CriterionOperator.IsNull, null);
        }

        /// <summary>
        /// Field is null
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery IsNull<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            return IsNull(sourceQuery, ExpressionHelper.GetExpressionPropertyName(field.Body), connector);
        }
    }
}
