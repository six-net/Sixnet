using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command execution context
    /// </summary>
    public partial class DataCommandExecutionContext
    {
        /// <summary>
        /// Get command table name
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return table name</returns>
        public Task<List<string>> GetTableNamesAsync(DataCommand command = null)
        {
            if (command != null)
            {
                SetCommand(command);
            }
            return GetTableNamesAsync(Command.Queryable, QueryableLocation.Top);
        }

        /// <summary>
        /// Get entity table names
        /// </summary>
        /// <param name="activityQueryable">Activity query</param>
        /// <param name="queryableLocation">Activity query location</param>
        /// <returns>Return table name</returns>
        public Task<List<string>> GetTableNamesAsync(ISixnetQueryable activityQueryable, QueryableLocation queryableLocation)
        {
            if (Command == null)
            {
                throw new SixnetException($"Data command is null");
            }
            SetActivityQueryable(activityQueryable, queryableLocation);
            return DataManager.GetTableNamesAsync(this);
        }
    }
}
