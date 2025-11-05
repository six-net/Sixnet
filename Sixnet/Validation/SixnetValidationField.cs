// "Company © 2025. All rights reserved."

namespace Sixnet.Validation
{
    /// <summary>
    /// Validation field
    /// </summary>
    public class SixnetValidationField<T>
    {
        /// <summary>
        /// Gets or sets the field
        /// </summary>
        public Expression<Func<T, dynamic>> Field { get; set; }

        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the compare value
        /// </summary>
        internal dynamic CompareValue { get; set; }

        /// <summary>
        /// Gets or sets the tip message
        /// </summary>
        public bool TipMessage { get; set; }

        /// <summary>
        /// Ignore use scenarios
        /// </summary>
        public IEnumerable<string> IgnoreUseScenarios { get; set; }
    }
}
