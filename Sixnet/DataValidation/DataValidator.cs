using System.ComponentModel.DataAnnotations;

namespace Sixnet.DataValidation
{
    /// <summary>
    /// Data validator
    /// </summary>
    public abstract class DataValidator
    {
        #region Fields

        /// <summary>
        /// Wheather pass validation
        /// </summary>
        protected bool isValidValue = false;

        /// <summary>
        /// Verify result
        /// </summary>
        protected VerifyResult verifyResult = null;

        /// <summary>
        /// Default error message
        /// </summary>
        protected string defaultErrorMessageValue = string.Empty;

        #endregion

        #region Properties

        /// <summary>
        /// Gets wheather pass validation
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValidValue;
            }
        }

        /// <summary>
        /// Gets the verify result
        /// </summary>
        public VerifyResult Result
        {
            get
            {
                return verifyResult;
            }
        }

        /// <summary>
        /// Gets the default error message
        /// </summary>
        public string DefaultErrorMessage
        {
            get
            {
                return defaultErrorMessageValue;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate data
        /// </summary>
        /// <param name="value">Validate value</param>
        /// <param name="errorMessage">Error message</param>
        public abstract void Validate(object value, string errorMessage);

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

        /// <summary>
        /// Set verify result
        /// </summary>
        /// <param name="isValid">Is valid</param>
        /// <param name="message">Message</param>
        protected void SetVerifyResult(bool isValid, string message = "")
        {
            this.isValidValue = isValid;
            verifyResult = this.isValidValue ? VerifyResult.SuccessResult() : VerifyResult.ErrorResult(FormatMessage(message));
        }

        #endregion
    }
}
