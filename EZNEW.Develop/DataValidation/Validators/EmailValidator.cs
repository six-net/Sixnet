using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// email validator
    /// </summary>
    public class EmailValidator : DataValidator
    {
        private static Regex _regex = CreateRegEx();
        public EmailValidator()
        {
            _errorMessage = "incorrect email format";
        }

        /// <summary>
        /// validate
        /// </summary>
        /// <param name="value">validate value</param>
        /// <param name="errorMessage">error message</param>
        public override void Validate(dynamic value, string errorMessage = "")
        {
            if (value == null)
            {
                SetVerifyResult(true);
                return;
            }
            string valueAsString = value as string;
            if (_regex != null)
            {
                _isValid = valueAsString != null && _regex.Match(valueAsString).Length > 0;
            }
            else
            {
                int atCount = 0;
                foreach (char c in valueAsString)
                {
                    if (c == '@')
                    {
                        atCount++;
                    }
                }
                _isValid = (valueAsString != null && atCount == 1 && valueAsString[0] != '@' && valueAsString[valueAsString.Length - 1] != '@');
            }
            SetVerifyResult(_isValid, errorMessage);
        }

        /// <summary>
        /// create validation attribute
        /// </summary>
        /// <returns></returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new EmailAddressAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }

        /// <summary>
        /// create regex
        /// </summary>
        /// <returns></returns>
        private static Regex CreateRegEx()
        {
            const string pattern = @"^((([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+(\.([a-z]|\d|[!#\$%&'\*\+\-\/=\?\^_`{\|}~]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])+)*)|((\x22)((((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(([\x01-\x08\x0b\x0c\x0e-\x1f\x7f]|\x21|[\x23-\x5b]|[\x5d-\x7e]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(\\([\x01-\x09\x0b\x0c\x0d-\x7f]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF]))))*(((\x20|\x09)*(\x0d\x0a))?(\x20|\x09)+)?(\x22)))@((([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|\d|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.)+(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])|(([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])([a-z]|\d|-|\.|_|~|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])*([a-z]|[\u00A0-\uD7FF\uF900-\uFDCF\uFDF0-\uFFEF])))\.?$";
            const RegexOptions options = RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture;
            TimeSpan matchTimeout = TimeSpan.FromSeconds(2);
            try
            {
                if (AppDomain.CurrentDomain.GetData("REGEX_DEFAULT_MATCH_TIMEOUT") == null)
                {
                    return new Regex(pattern, options, matchTimeout);
                }
            }
            catch
            {
            }
            return new Regex(pattern, options);
        }
    }
}
