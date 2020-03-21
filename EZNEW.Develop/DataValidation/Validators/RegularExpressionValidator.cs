using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// regular expression validator
    /// </summary>
    public class RegularExpressionValidator : DataValidator
    {
        private int _matchTimeoutInMilliseconds;
        private bool _matchTimeoutSet;
        public string Pattern { get; private set; }
        public int MatchTimeoutInMilliseconds
        {
            get
            {
                return _matchTimeoutInMilliseconds;
            }
            set
            {
                _matchTimeoutInMilliseconds = value;
                _matchTimeoutSet = true;
            }
        }
        private Regex Regex { get; set; }
        public RegularExpressionValidator(string pattern)
        {
            this.Pattern = pattern;
        }
        public override void Validate(dynamic value, string errorMessage)
        {
            this.SetupRegex();
            string stringValue = Convert.ToString(value, CultureInfo.CurrentCulture);
            if (String.IsNullOrEmpty(stringValue))
            {
                SetVerifyResult(true, errorMessage);
                return;
            }
            Match m = this.Regex.Match(stringValue);
            SetVerifyResult((m.Success && m.Index == 0 && m.Length == stringValue.Length), errorMessage);
        }

        void SetupRegex()
        {
            Regex = new Regex(Pattern, default(RegexOptions), TimeSpan.FromMilliseconds((double)MatchTimeoutInMilliseconds));
        }

        /// <summary>
        /// create validation attribute
        /// </summary>
        /// <returns></returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new RegularExpressionAttribute(Pattern)
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
