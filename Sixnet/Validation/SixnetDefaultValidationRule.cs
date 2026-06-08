// "Company © 2025. All rights reserved."

namespace Sixnet.Validation
{
    /// <summary>
    /// Default validation rule
    /// </summary>
    public class SixnetDefaultValidationRule<T> : ISixnetValidationRule<T>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        public SixnetValidationField<T> Field { get; set; }

        public static SixnetDefaultValidationRule<T> Create(Expression<Func<T, dynamic>> field, string displayName = null)
        {
            return new SixnetDefaultValidationRule<T>()
            {
                Field = new SixnetValidationField<T>()
                {
                    Field = field,
                    Display = displayName
                }
            };
        }
    }
}
