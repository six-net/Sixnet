using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.CriteriaConvert
{
    /// <summary>
    /// string length criteria convert
    /// </summary>
    public class StringLengthCriteriaConvert : ICriteriaConvert
    {
        /// <summary>
        /// convert type
        /// </summary>
        public CriteriaConvertType Type { get; set; } = CriteriaConvertType.StringLength;
    }
}
