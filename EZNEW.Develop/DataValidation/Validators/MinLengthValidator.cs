using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// min length validator
    /// </summary>
    public class MinLengthValidator : DataValidator
    {
        public int Length { get; private set; }

        public MinLengthValidator(int length)
        {
            Length = length;
            _errorMessage = "the value is less than the minimum length";
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
            _isValid = length >= Length;
            SetVerifyResult(_isValid, errorMessage);
        }

        private void EnsureLegalLengths()
        {
            if (Length < 0)
            {
                throw new InvalidOperationException("MinLength Value Is Error");
            }
        }

        /// <summary>
        /// create validation attribute
        /// </summary>
        /// <returns></returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new MinLengthAttribute(Length)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
