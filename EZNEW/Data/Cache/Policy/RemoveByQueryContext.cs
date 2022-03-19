using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Command;
using EZNEW.Development.Query;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Remove by query context
    /// </summary>
    public class RemoveByQueryContext<T>
    {
        /// <summary>
        /// Gets or sets the database command
        /// </summary>
        public ICommand DatabaseCommand { get; set; }

        /// <summary>
        /// Gets or sets the query
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the datas
        /// </summary>
        public IEnumerable<T> Datas { get; set; }
    }
}
