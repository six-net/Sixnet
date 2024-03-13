using Sixnet.Development.Data.Field;
using Sixnet.Development.Data.Field.Formatting;
using Sixnet.Development.Queryable;
using Sixnet.Expressions.Linq;

namespace System.Linq.Expressions
{
    public static class ExpressionExtensions
    {
        /// <summary>
        /// Gets the last member name
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static string GetLastMemberName(this Expression expression)
        {
            return SixnetExpressionHelper.GetExpressionLastPropertyName(SixnetExpressionHelper.GetLastChildExpression(expression));
        }

        /// <summary>
        /// Get queryable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static ISixnetQueryable GetQueryable<T>(this Expression expression)
        {
            return SixnetExpressionHelper.GetQueryable<T>(expression);
        }

        /// <summary>
        /// Get fields assignment
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static FieldsAssignment GetFieldsAssignment(this Expression expression)
        {
            return SixnetExpressionHelper.GetFieldsAssignment(expression);
        }

        /// <summary>
        /// Get data field
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        internal static ISixnetField GetDataField(this Expression expression, string formatterName = "")
        {
            var dataField = SixnetExpressionHelper.GetDataField(expression);
            if (dataField != null && !string.IsNullOrWhiteSpace(formatterName))
            {
                dataField.FormatSetting = FieldFormatSetting.Create(formatterName);
            }
            return dataField;
        }
    }
}
