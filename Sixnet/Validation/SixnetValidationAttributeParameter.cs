// "Company © 2025. All rights reserved."

namespace Sixnet.Validation
{
    /// <summary>
    /// Validation attribute parameter
    /// </summary>
    [Serializable]
    public class SixnetValidationAttributeParameter
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the other property
        /// </summary>
        public string OtherProperty { get; set; }
    }
}
