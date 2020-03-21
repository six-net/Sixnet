using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    public class RequiredValidator : DataValidator
    {
        public RequiredValidator(bool allowEmptyStrings=false)
        {
            AllowEmptyStrings = allowEmptyStrings;
        }
        public bool AllowEmptyStrings { get; set; }

        public override void Validate(dynamic value, string errorMessage)
        {
            if (value == null)
            {
                SetVerifyResult(false, errorMessage);
                return;
            }
            var stringValue = value as string;
            if (stringValue != null && !AllowEmptyStrings)
            {
                SetVerifyResult(stringValue.Trim().Length != 0,errorMessage);
                return;
            }
            SetVerifyResult(true);
        }

        /// <summary>
        /// create validation attribute
        /// </summary>
        /// <returns></returns>
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            return new RequiredAttribute()
            {
                ErrorMessage = FormatMessage(parameter.ErrorMessage)
            };
        }
    }
}
