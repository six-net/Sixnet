using System;
using System.Collections.Generic;
using Sixnet.Development.Data;
using Sixnet.Development.Data.Database;
using Sixnet.Development.Queryable;
using Sixnet.Exceptions;

namespace Sixnet.Development.Data.Command
{
    /// <summary>
    /// Defines data command execution context
    /// </summary>
    public partial class DataCommandExecutionContext
    {
        #region Constructor

        private DataCommandExecutionContext(SixnetDatabaseConnection connection, SixnetDataCommand command)
        {
            Server = connection?.DatabaseServer ?? throw new ArgumentNullException(nameof(DatabaseConnection.DatabaseServer));
            DatabaseConnection = connection;
            if (command != null)
            {
                SetCommand(command);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the server
        /// </summary>
        public SixnetDatabaseServer Server { get; private set; }

        /// <summary>
        /// Database connection
        /// </summary>
        public SixnetDatabaseConnection DatabaseConnection { get; private set; }

        /// <summary>
        /// Gets the data command
        /// </summary>
        public SixnetDataCommand Command { get; private set; }

        /// <summary>
        /// Gets the activity queryable
        /// </summary>
        public ISixnetQueryable ActivityQueryable { get; private set; }

        /// <summary>
        /// Gets or sets the activity queryable location
        /// </summary>
        public QueryableLocation QueryableLocation { get; private set; }

        #endregion

        #region Fields

        IEnumerable<dynamic> splitValues;

        #endregion

        #region Methods

        /// <summary>
        /// Get command table name
        /// </summary>
        /// <param name="command">Command</param>
        /// <returns>Return table name</returns>
        public List<string> GetTableNames(SixnetDataCommand command = null)
        {
            if (command != null)
            {
                SetCommand(command);
            }
            return GetTableNames(Command.Queryable, QueryableLocation.Top);
        }

        /// <summary>
        /// Get entity table names
        /// </summary>
        /// <param name="activityQueryable">Activity query</param>
        /// <param name="queryableLocation">Activity query location</param>
        /// <returns>Return table name</returns>
        public List<string> GetTableNames(ISixnetQueryable activityQueryable, QueryableLocation queryableLocation)
        {
            if (Command == null)
            {
                throw new SixnetException($"Data command is null");
            }
            SetActivityQueryable(activityQueryable, queryableLocation);
            return SixnetDataManager.GetTableNames(this);
        }

        /// <summary>
        /// Set activity query
        /// </summary>
        /// <param name="queryable">Queryable</param>
        /// <param name="location">Location</param>
        public void SetActivityQueryable(ISixnetQueryable queryable, QueryableLocation location)
        {
            ActivityQueryable = queryable;
            QueryableLocation = location;
        }

        /// <summary>
        /// Set command
        /// </summary>
        /// <param name="command">Command</param>
        public void SetCommand(SixnetDataCommand command)
        {
            SixnetDirectThrower.ThrowArgNullIf(command == null, nameof(command));

            Command = command;
            SetActivityQueryable(command.Queryable, QueryableLocation.Top);
        }

        /// <summary>
        /// Sets the split values
        /// </summary>
        /// <param name="splitValues">Split values</param>
        public void SetSplitValues(IEnumerable<dynamic> splitValues)
        {
            this.splitValues = splitValues;
        }

        /// <summary>
        /// Gets the split values
        /// </summary>
        /// <returns></returns>
        public SplitTableBehavior GetSplitTableBehavior()
        {
            if (Command == null)
            {
                return null;
            }
            switch (Command.OperationType)
            {
                case DataOperationType.Insert:
                    return new SplitTableBehavior()
                    {
                        SelectionPattern = SplitTableNameSelectionPattern.Precision,
                        SplitValues = splitValues
                    };
                case DataOperationType.BulkInsert:
                    throw new NotSupportedException($"Not support get split values for {DataOperationType.BulkInsert}");
                default:
                    return ActivityQueryable?.SplitTableBehavior;
            }
        }

        /// <summary>
        /// Create data command execution context
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns></returns>
        public static DataCommandExecutionContext Create(SixnetDatabaseConnection connection, SixnetDataCommand command = null)
        {
            return new DataCommandExecutionContext(connection, command)
            {
                ActivityQueryable = command?.Queryable,
                QueryableLocation = QueryableLocation.Top
            };
        }

        #endregion
    }
}
