using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Sixnet.DataValidation
{
    /// <summary>
    /// Defines default validation rule
    /// </summary>
    public class ValidationRule<T> : IValidationRule<T>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        public ValidationField<T> Field { get; set; }

        public static ValidationRule<T> Create(Expression<Func<T, dynamic>> field)
        {
            return new ValidationRule<T>()
            {
                Field = new ValidationField<T>()
                {
                    Field = field
                }
            };
        }
    }
}
