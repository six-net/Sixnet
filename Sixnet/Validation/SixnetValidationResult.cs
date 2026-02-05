// "Company © 2025. All rights reserved."

using Sixnet.Localization;

namespace Sixnet.Validation
{
    /// <summary>
    /// Validation result
    /// </summary>
    [Serializable]
    public class SixnetValidationResult
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether verify successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the field name
        /// </summary>
        public string FieldName { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets error result
        /// </summary>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Return verify result</returns>
        public static SixnetValidationResult ErrorResult(string errorMessage = "", IEnumerable<string> args = null)
        {
            return new SixnetValidationResult()
            {
                Success = false,
                Message = SixnetLocalizer.GetString(errorMessage, args?.ToArray())
            };
        }

        /// <summary>
        /// Gets success result
        /// </summary>
        /// <param name="successMessage">Success message</param>
        /// <returns>Return verify result</returns>
        public static SixnetValidationResult SuccessResult(string successMessage = "")
        {
            return new SixnetValidationResult()
            {
                Success = true,
                Message = SixnetLocalizer.GetString(successMessage)
            };
        }

        #endregion
    }
}
