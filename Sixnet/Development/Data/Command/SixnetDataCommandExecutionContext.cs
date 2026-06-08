// "Company © 2025. All rights reserved."

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
        #region Constructor

        private SixnetDataCommandExecutionContext(SixnetDatabaseConnection connection, SixnetDataCommand command)
        {
            Server = connection?.DatabaseServer;
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
        public SixnetQueryableLocation QueryableLocation { get; private set; }

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
        public List<SixnetDatabaseObjectName> GetTableNames(SixnetDataCommand command = null)
        {
            if (command != null)
            {
                SetCommand(command);
            }
            return GetTableNames(Command.Queryable, SixnetQueryableLocation.Top);
        }

        /// <summary>
        /// Get entity table names
        /// </summary>
        /// <param name="activityQueryable">Activity query</param>
        /// <param name="queryableLocation">Activity query location</param>
        /// <returns>Return table name</returns>
        public List<SixnetDatabaseObjectName> GetTableNames(ISixnetQueryable activityQueryable, SixnetQueryableLocation queryableLocation)
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
        public void SetActivityQueryable(ISixnetQueryable queryable, SixnetQueryableLocation location)
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
            SetActivityQueryable(command.Queryable, SixnetQueryableLocation.Top);
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
        public SixnetSplitTableBehavior GetSplitTableBehavior()
        {
            if (Command == null)
            {
                return null;
            }
            switch (Command.OperationType)
            {
                case SixnetDataOperationType.Insert:
                    return splitValues.IsNullOrEmpty() ? (Command.Options?.SplitTableBehavior ?? ActivityQueryable?.SplitTableBehavior) 
                                                        : new SixnetSplitTableBehavior()
                                                        {
                                                            SelectionPattern = SixnetSplitTableNameSelectionPattern.Precision,
                                                            SplitValues = splitValues
                                                        };
                case SixnetDataOperationType.BulkInsert:
                    throw new NotSupportedException($"Not support get split values for {SixnetDataOperationType.BulkInsert}");
                default:
                    return Command.Options?.SplitTableBehavior ?? ActivityQueryable?.SplitTableBehavior;
            }
        }

        /// <summary>
        /// Create data command execution context
        /// </summary>
        /// <param name="server">Database server</param>
        /// <param name="command">Command</param>
        /// <returns></returns>
        public static SixnetDataCommandExecutionContext Create(SixnetDatabaseConnection connection, SixnetDataCommand command = null)
        {
            return new SixnetDataCommandExecutionContext(connection, command)
            {
                ActivityQueryable = command?.Queryable,
                QueryableLocation = SixnetQueryableLocation.Top
            };
        }

        #endregion
    }
}
