using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Modify data by condition
    /// </summary>
    public class ModifyDataByConditionCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the new data
        /// </summary>
        public T NewData { get; set; }

        /// <summary>
        /// Gets or sets the old data
        /// </summary>
        public T OldData { get; set; }

        /// <summary>
        /// Gets or sets the modify database data func
        /// </summary>
        public Func<T, T, IQuery, ICommand> ModifyDatabaseDataProxy { get; set; }

        /// <summary>
        /// Gets or sets the get database data list func
        /// </summary>
        public Func<IQuery, List<T>> GetDataListProxy { get; set; }
    }
}
