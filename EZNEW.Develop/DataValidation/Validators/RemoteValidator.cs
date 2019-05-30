using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    public class RemoteValidator : DataValidator
    {
        public override ValidationAttribute CreateValidationAttribute(ValidationAttributeParameter parameter)
        {
            throw new NotImplementedException();
        }

        public override void Validate(dynamic value, string errorMessage)
        {
            throw new NotImplementedException();
        }
    }
}
