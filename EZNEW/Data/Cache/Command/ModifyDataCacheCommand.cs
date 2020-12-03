using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Modify data cache command
    /// </summary>
    /// <typeparam name="T">Data type</typeparam>
    public class ModifyDataCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets the new data
        /// </summary>
        public T NewData { get; set; }

        /// <summary>
        /// Gets or sets the old data
        /// </summary>
        public T OldData { get; set; }

        /// <summary>
        /// Gets or sets modify database data function
        /// </summary>
        public Func<T, T, ICommand> ModifyDatabaseDataProxy { get; set; }
    }
}
