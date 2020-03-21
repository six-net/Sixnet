using EZNEW.Develop.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

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

        public static readonly CommandExecuteOption Default = new CommandExecuteOption();
    }
}
