using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Development.Command;
using EZNEW.Development.Command.Modification;
using EZNEW.Development.Query;

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
        public IModification Modify { get; set; }

        /// <summary>
        /// Gets or sets the modify database data func
        /// </summary>
        public Func<IModification, IQuery, ICommand> ModifyDatabaseDataProxy { get; set; }

        /// <summary>
        /// Gets or sets the get database data list func
        /// </summary>
        public Func<IQuery, List<T>> GetDataListProxy { get; set; }
    }
}
