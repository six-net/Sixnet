using System.ComponentModel.DataAnnotations;

namespace Sixnet.Validation
{
    /// <summary>
    /// Sixnet validator
    /// </summary>
    public abstract class SixnetValidator
    {
        #region Fields

        /// <summary>
        /// Default error message
        /// </summary>
        protected string defaultErrorMessageValue = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the default error message
        /// </summary>
        public string DefaultErrorMessage => defaultErrorMessageValue;

        #endregion

        #region Methods

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="data">Data</param>
        /// <param name="errorMessage">Error message</param>
        public abstract ValidationResult Validate(object data, string errorMessage);

        /// <summary>
        /// Create validation attribute
        /// </summary>
        /// <param name="parameter">Parameter</param>
        /// <returns>Return the validation attribute</returns>
        public abstract ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter);

        /// <summary>
        /// Format message
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Return the formated message</returns>
        protected string FormatMessage(string errorMessage)
        {
            return string.IsNullOrWhiteSpace(errorMessage) ? defaultErrorMessageValue : errorMessage;
        }

        #endregion
    }
}
