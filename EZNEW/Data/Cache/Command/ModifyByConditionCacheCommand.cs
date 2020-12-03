using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command;
using EZNEW.Develop.Command.Modify;
using EZNEW.Develop.CQuery;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Modify by condition cache command
    /// </summary>
    public class ModifyByConditionCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets the query condition
        /// </summary>
        public IQuery Query { get; set; }

        /// <summary>
        /// Gets or sets the modify object
        /// </summary>
        public IModify Modify { get; set; }

        /// <summary>
        /// Gets or sets the modify database data func
        /// </summary>
        public Func<IModify, IQuery, ICommand> ModifyDatabaseDataProxy { get; set; }

        /// <summary>
        /// Gets or sets the get database data list func
        /// </summary>
        public Func<IQuery, List<T>> GetDataListProxy { get; set; }
    }
}
