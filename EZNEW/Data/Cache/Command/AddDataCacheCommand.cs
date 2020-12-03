using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EZNEW.Develop.Command;

namespace EZNEW.Data.Cache.Command
{
    /// <summary>
    /// Add data cache command
    /// </summary>
    public class AddDataCacheCommand<TData>
    {
        /// <summary>
        /// Gets or sets the data
        /// </summary>
        public TData Data { get; set; }

        /// <summary>
        /// Gets or sets the add data to database function
        /// </summary>
        public Func<TData, ICommand> AddDataToDatabaseProxy { get; set; }

        /// <summary>
        /// Gets or sets the expire time
        /// </summary>
        public TimeSpan? Expire { get; set; }
    }
}
