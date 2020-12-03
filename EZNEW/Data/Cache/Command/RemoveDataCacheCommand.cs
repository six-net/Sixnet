using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Remove data cache command
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RemoveDataCacheCommand<T>
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Gets or sets the remove database data func
        /// </summary>
        public Func<T, ICommand> RemoveDatabaseDataProxy { get; set; }
    }
}
