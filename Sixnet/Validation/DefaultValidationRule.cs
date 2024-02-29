using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sixnet.Validation
{
    /// <summary>
    /// Default validation rule
    /// </summary>
    public class DefaultValidationRule<T> : ISixnetValidationRule<T>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        public ValidationField<T> Field { get; set; }

        public static DefaultValidationRule<T> Create(Expression<Func<T, dynamic>> field)
        {
            return new DefaultValidationRule<T>()
            {
                Field = new ValidationField<T>()
                {
                    Field = field
                }
            };
        }
    }
}
