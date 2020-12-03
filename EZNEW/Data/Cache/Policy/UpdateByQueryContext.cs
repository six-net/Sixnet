using System;
using System.Collections.Generic;
using System.Text;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data.Cache.Policy
{
    /// <summary>
    /// Update by query context
    /// </summary>
    public class UpdateByQueryContext<T>
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
