using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.DataValidation.Validators
{
    /// <summary>
    /// compare operator value
    /// </summary>
    public class CompareOperatorValue
    {
        #region propertys

        /// <summary>
        /// source value
        /// </summary>
        public dynamic SourceValue
        {
            get;set;
        }

        /// <summary>
        /// compare value
        /// </summary>
        public dynamic CompareValue
        {
            get;set;
        }

        #endregion
    }
}
