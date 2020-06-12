using System;

namespace EZNEW.DataValidation
{
    /// <summary>
    /// Validation attribute parameter
    /// </summary>
    [Serializable]
    public class ValidationAttributeParameter
    {
        /// <summary>
        /// Gets or sets the error message
        /// </summary>
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the other property
        /// </summary>
        public string OtherProperty
        {
            get; set;
        }
    }
}
