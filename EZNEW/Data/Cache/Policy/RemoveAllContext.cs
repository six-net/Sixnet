using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Development.Command;
using EZNEW.Development.Query;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Remove all context
    /// </summary>
    public class RemoveAllContext<T>
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
        /// Gets or sets the get datas proxy
        /// </summary>
        public Func<IQuery, List<T>> GetDatasProxy { get; set; }
    }
}
