using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    public interface IValidation
    {
        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="obj">validate object</param>
        /// <returns></returns>
        VerifyResult Validate(dynamic obj);

        /// <summary>
        /// Create Validation Attribute
        /// </summary>
        /// <returns></returns>
        ValidationAttribute CreateValidationAttribute();
    }
}
