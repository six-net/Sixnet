// "Company © 2025. All rights reserved."

using System.Threading;

namespace Sixnet.Development.Data.Database
{
    /// <summary>
    /// Defines database command
    /// </summary>
    public class SixnetDatabaseCommand
    {
        public SixnetDatabaseCommand() { }

        /// <summary>
        /// Gets or set the database connection
        /// </summary>
        public SixnetDatabaseConnection Connection { get; set; }

        /// <summary>
        /// Gets or sets the cancellation token
        /// </summary>
        public CancellationToken? CancellationToken { get; set; }

        /// <summary>
        /// Command timeout (in seconds)
        /// </summary>
        public int? CommandTimeout { get; set; }

        public static T Create<T>(SixnetDatabaseConnection connection, SixnetDataOperationOptions options, Action<T> configure = null) where T : SixnetDatabaseCommand, new()
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

        public static SixnetDatabaseCommand Create(SixnetDatabaseConnection connection, SixnetDataOperationOptions options, Action<SixnetDatabaseCommand> configure = null)
        {
            return Create<SixnetDatabaseCommand>(connection, options, configure);
        }
    }
}
