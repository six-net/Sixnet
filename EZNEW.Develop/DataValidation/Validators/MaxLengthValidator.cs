using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// max length validator
    /// </summary>
    public class MaxLengthValidator : DataValidator
    {
        private const int MaxAllowableLength = -1;

        public int Length { get; private set; }

        public MaxLengthValidator(int length)
        {
            _errorMessage = "the value exceeds the maximum length";
            Length = length;
        }

        public override void Validate(dynamic value, string errorMessage)
        {
            EnsureLegalLengths();
            var length = 0;
            if (value == null)
            {
                SetVerifyResult(true, errorMessage);
                return;
            }
            else
            {
                var str = value as string;
                if (str != null)
                {
                    length = str.Length;
                }
                else
                {
                    length = ((Array)value).Length;
                }
            }
            _isValid = MaxAllowableLength == Length || length <= Length;
            SetVerifyResult(_isValid, errorMessage);
        }

        private void EnsureLegalLengths()
        {
            if (Length == 0 || Length < -1)
            {
                throw new InvalidOperationException("Max Length Value Is Error");
            }
        }

        /// <summary>
        /// create validation attribute
        /// </summary>
        /// <returns></returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new MaxLengthAttribute(Length)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
