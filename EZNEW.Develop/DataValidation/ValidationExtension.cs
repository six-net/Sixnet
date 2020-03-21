using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation
{
    public static class ValidationExtension
    {
        /// <summary>
        /// get error message
        /// </summary>
        /// <param name="results">results</param>
        /// <returns></returns>
        public static string[] GetErrorMessage(this IEnumerable<VerifyResult> results)
        {
            if (results == null)
            {
                return new string[0];
            }
            List<string> errorMessages = new List<string>();
            foreach (var result in results)
            {
                if (!result.Success)
                {
                    errorMessages.Add(string.Format("{0}/{1}",result.FieldName,result.ErrorMessage));
                }
            }
            return errorMessages.ToArray();
        }
    }
}
