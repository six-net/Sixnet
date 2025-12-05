// "Company © 2025. All rights reserved."

using System.Threading;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database command
    /// </summary>
    public class DatabaseCommand
    {
        public DatabaseCommand() { }

        /// <summary>
        /// Gets or set the database connection
        /// </summary>
        public DatabaseConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }

        /// <summary>
        /// Command timeout (in seconds)
        /// </summary>
        public int? CommandTimeout { get; set; }

        public static T Create<T>(DatabaseConnection connection, SixnetDataOperationOptions options, Action<T> configure = null) where T : DatabaseCommand, new()
        {
            var cmd = new T()
            {
                Connection = connection,
                CancellationToken = options?.CancellationToken,
                CommandTimeout = SixnetDataManager.GetCommandTimeout(options)
            };
            configure?.Invoke(cmd);
            return cmd;
        }

        public static DatabaseCommand Create(DatabaseConnection connection, SixnetDataOperationOptions options, Action<DatabaseCommand> configure = null)
        {
            return Create<DatabaseCommand>(connection, options, configure);
        }
    }
}
