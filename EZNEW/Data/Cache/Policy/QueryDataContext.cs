using EZNEW.Develop.CQuery;
using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Query data context
    /// </summary>
    public class QueryDataContext<T>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public IQuery Query { get; set; }
    }
}
