using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Check callback context
    /// </summary>
    public class CheckDataCallbackContext<T>
    {
        /// <summary>
        /// Whether has value
        /// </summary>
        public bool HasValue { get; set; }

        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public IQuery Query { get; set; }
    }
}
