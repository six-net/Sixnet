using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.CQuery.CriteriaConvert
{
    /// <summary>
    /// default criteria convert
    /// </summary>
    public class DefaultCriteriaConvert : ICriteriaConvert
    {
        /// <summary>
        /// convert name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// data
        /// </summary>
        public object Data { get; set; }
    }
}
