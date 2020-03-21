using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// validation field
    /// </summary>
    public class ValidationField<T>
    {
        /// <summary>
        /// field expression
        /// </summary>
        public Expression<Func<T, dynamic>> FieldExpression
        {
            get; set;
        }

        /// <summary>
        /// error message
        /// </summary>
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// compare value
        /// </summary>
        internal dynamic CompareValue
        {
            get; set;
        }

        /// <summary>
        /// tip message
        /// </summary>
        public bool TipMessage
        {
            get; set;
        }
    }
}
