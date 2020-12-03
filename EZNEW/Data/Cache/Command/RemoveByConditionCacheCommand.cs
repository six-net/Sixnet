using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Remove by condition cache command
    /// </summary>
    public class RemoveByConditionCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the remove database data func
        /// </summary>
        public Func<IQuery, ICommand> RemoveDatabaseDataProxy { get; set; }

        /// <summary>
        /// Gets or sets the get database data list func
        /// </summary>
        public Func<IQuery, List<T>> GetDataListProxy { get; set; }
    }
}
