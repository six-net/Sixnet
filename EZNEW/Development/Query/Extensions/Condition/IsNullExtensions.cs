﻿using System;
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
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery IsNull(this IQuery sourceQuery, string fieldName, bool or = false)
        {
            return sourceQuery.AddCriterion(or ? CriterionConnectionOperator.Or : CriterionConnectionOperator.And, fieldName, CriterionOperator.IsNull, null);
        }

        /// <summary>
        /// Field is null
        /// </summary>
        /// <typeparam name="TQueryModel">Query model</typeparam>
        /// <param name="sourceQuery">Source query</param>
        /// <param name="field">Field</param>
        /// <param name="or">Connection with 'and'(true/default) or 'or'(false)</param>
        /// <returns>Return the newest IQuery object</returns>
        public static IQuery IsNull<TQueryModel>(this IQuery sourceQuery, Expression<Func<TQueryModel, dynamic>> field, bool or = false) where TQueryModel : IQueryModel<TQueryModel>
        {
            return sourceQuery.AddCriterion(or ? CriterionConnectionOperator.Or : CriterionConnectionOperator.And, ExpressionHelper.GetExpressionPropertyName(field.Body), CriterionOperator.IsNull, null);
        }
    }
}
