using System;
using System.Threading;
using EZNEW.Develop.DataAccess;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command execute options
    /// </summary>
    [Serializable]
    public class CommandExecuteOptions
    {
        /// <summary>
        /// Gets or sets the data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// Gets or sets whether execute by transaction
        /// </summary>
        public bool ExecuteByTransaction { get; set; } = true;

        /// <summary>
        /// Gets or sets the cancellation token
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = default;

        /// <summary>
        /// Gets the default command execute option
        /// </summary>
        public static readonly CommandExecuteOptions Default = new CommandExecuteOptions();
    }
}
