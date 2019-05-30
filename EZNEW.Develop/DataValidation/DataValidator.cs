using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    /// <summary>
    /// Data Validator
    /// </summary>
    public abstract class DataValidator
    {
        #region fields

        /// <summary>
        /// wheather pass validation
        /// </summary>
        protected bool _isValid = false;
        /// <summary>
        /// verify result
        /// </summary>
        protected VerifyResult _verifyResult = null;
        /// <summary>
        /// error message
        /// </summary>
        protected string _errorMessage = string.Empty;

        #endregion

        #region Propertys

        /// <summary>
        /// wheather pass validation
        /// </summary>
        public bool IsValid
        {
            get
            {
                return _isValid;
            }
        }

        /// <summary>
        /// Verify Result
        /// </summary>
        public VerifyResult Result
        {
            get
            {
                return _verifyResult;
            }
        }

        /// <summary>
        /// Default Error Message
        /// </summary>
        public string DefaultErrorMessage
        {
            get
            {
                return _errorMessage;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="value">Validate Value</param>
        /// <param name="errorMessage">Error Message</param>
        public abstract void Validate(dynamic value,string errorMessage);

        /// <summary>
        /// Create Validation Attribute
        /// </summary>
        /// <param name="parameter">parameter</param>
        /// <returns></returns>
        public abstract ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter);

        /// <summary>
        /// Format Message
        /// </summary>
        /// <param name="errorMessage">error message</param>
        /// <returns></returns>
        protected string FormatMessage(string errorMessage)
        {
            return string.IsNullOrWhiteSpace(errorMessage) ? _errorMessage : errorMessage;
        }

        /// <summary>
        /// Set Verify Result
        /// </summary>
        /// <param name="isValid">is valid</param>
        /// <param name="message">message</param>
        protected void SetVerifyResult(bool isValid, string message = "")
        {
            _isValid = isValid;
            _verifyResult = _isValid ? VerifyResult.SuccessResult() : VerifyResult.ErrorResult(FormatMessage(message));
        }

        #endregion
    }
}
