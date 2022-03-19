using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using EZNEW.Expressions;

namespace EZNEW.Development.Query
{
    public static class NotNullExtensions
    {
        /// <summary>
        /// Field is not null
        /// </summary>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="fieldName">Field name</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotNull(this IQuery sourceQuery, string fieldName, CriterionConnector connector = CriterionConnector.And)
        {
            return sourceQuery.AddCriterion(connector, FieldInfo.Create(fieldName), CriterionOperator.NotNull, null);
        }

        /// <summary>
        /// Field is not null
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="connector">Connector</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery NotNull<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, CriterionConnector connector = CriterionConnector.And) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddCriterion(connector, FieldInfo.Create(ExpressionHelper.GetExpressionPropertyName(field.Body)), CriterionOperator.NotNull, null);
        }
    }
}
