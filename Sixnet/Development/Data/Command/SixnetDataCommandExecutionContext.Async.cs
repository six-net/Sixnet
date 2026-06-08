// "Company © 2025. All rights reserved."

using System.Threading.Tasks;

using Sixnet.Development.Data.Database;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command execution context
    /// </summary>
    public partial class SixnetDataCommandExecutionContext
    {
        /// <summary>
        /// Get command table name
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return table name</returns>
        public Task<List<SixnetDatabaseObjectName>> GetTableNamesAsync(SixnetDataCommand command = null)
        {
            if (command != null)
            {
                SetCommand(command);
            }
            return GetTableNamesAsync(Command.Queryable, SixnetQueryableLocation.Top);
        }

        /// <summary>
        /// Get entity table names
        /// </summary>
        /// <param name="activityQueryable">Activity query</param>
        /// <param name="queryableLocation">Activity query location</param>
        /// <returns>Return table name</returns>
        public Task<List<SixnetDatabaseObjectName>> GetTableNamesAsync(ISixnetQueryable activityQueryable, SixnetQueryableLocation queryableLocation)
        {
            if (Command == null)
            {
                throw new SixnetException($"Data command is null");
            }
            SetActivityQueryable(activityQueryable, queryableLocation);
            return SixnetDataManager.GetTableNamesAsync(this);
        }
    }
}
