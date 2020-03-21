using EZNEW.Develop.CQuery.CriteriaConvert;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.CQuery
{
    /// <summary>
    /// order expression
    /// </summary>
    public class OrderCriteria
    {
        #region property

        /// <summary>
        /// field
        /// </summary>
        public string Name
        {
            get;set;
        }

        /// <summary>
        /// order by desc
        /// </summary>
        public bool Desc
        {
            get;set;
        }

        /// <summary>
        /// criteria convert
        /// </summary>
        public ICriteriaConvert Convert
        {
            get;set;
        }

        #endregion
    }
}
