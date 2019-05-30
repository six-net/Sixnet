using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// String Length Validator
    /// </summary>
    public class StringLengthValidator : DataValidator
    {
        public StringLengthValidator(int maxLength, int minLength = 0)
        {
            MaximumLength = maxLength;
            MinimumLength = minLength;
            _errorMessage = string.Format("the character length is between {0} and {1}", minLength, maxLength);
        }

        #region 属性

        /// <summary>
        /// maxlength
        /// </summary>
        public int MaximumLength { get; }
        /// <summary>
        /// minlength
        /// </summary>
        public int MinimumLength { get; set; }

        #endregion

        #region 方法

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="value">Validate Value</param>
        /// <param name="errorMessage">Error Message</param>
        public override void Validate(dynamic value, string errorMessage = "")
        {
            EnsureLegalLengths();
            int length = value == null ? 0 : ((string)value).Length;
            _isValid = value == null || (length >= this.MinimumLength && length <= this.MaximumLength);
            SetVerifyResult(_isValid, errorMessage);
        }

        /// <summary>
        /// EnsureLegalLengths
        /// </summary>
        private void EnsureLegalLengths()
        {
            if (this.MaximumLength < 0)
            {
                throw new InvalidOperationException("MaximumLength Is Less 0");
            }

            if (this.MaximumLength < this.MinimumLength)
            {
                throw new InvalidOperationException("MaximumLength Is Less Than MinimumLength Value");
            }
        }

        /// <summary>
        /// Create Validation Attribute
        /// </summary>
        /// <returns></returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new StringLengthAttribute(MaximumLength)
            {
                MinimumLength = MinimumLength,
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        #endregion
    }
}
