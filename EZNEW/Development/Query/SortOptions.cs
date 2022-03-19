using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Data.Conversion;

namespace EZNEW.Development.Query
{
    /// <summary>
    /// Defines sort options
    /// </summary>
    public class SortOptions : IInnerClone<SortOptions>
    {
        /// <summary>
        /// Clone a new criterion options
        /// </summary>
        /// <returns></returns>
        public SortOptions Clone()
        {
            return new SortOptions();
        }
    }
}
