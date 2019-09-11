using System;
using System.Collections.Generic;
using System.Text;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command before execute result
    /// </summary>
    public class CommandBeforeExecuteResult
    {
        /// <summary>
        /// allow execute command
        /// </summary>
        public bool AllowExecuteCommand
        {
            get;set;
        }

        public readonly static CommandBeforeExecuteResult DefaultSuccess = new CommandBeforeExecuteResult()
        {
            AllowExecuteCommand = true
        };
    }
}
