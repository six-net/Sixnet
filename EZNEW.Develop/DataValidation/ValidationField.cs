using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// Validation Field
    /// </summary>
    public class ValidationField<T>
    {
        /// <summary>
        /// Field Expression
        /// </summary>
        public Expression<Func<T, dynamic>> FieldExpression
        {
            get; set;
        }

        /// <summary>
        /// Error Message
        /// </summary>
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// Compare Value
        /// </summary>
        internal dynamic CompareValue
        {
            get; set;
        }

        /// <summary>
        /// Tip Message
        /// </summary>
        public bool TipMessage
        {
            get; set;
        }
    }
}
