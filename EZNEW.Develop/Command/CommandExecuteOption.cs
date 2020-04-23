using EZNEW.Develop.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command execute option
    /// </summary>
    public class CommandExecuteOption
    {
        /// <summary>
        /// data isolation level
        /// </summary>
        public DataIsolationLevel? IsolationLevel { get; set; }

        /// <summary>
        /// execute by transaction
        /// </summary>
        public bool ExecuteByTransaction { get; set; } = true;

        /// <summary>
        /// cancellation token
        /// </summary>
        public CancellationToken CancellationToken { get; set; } = default;

        public static readonly CommandExecuteOption Default = new CommandExecuteOption();
    }
}
