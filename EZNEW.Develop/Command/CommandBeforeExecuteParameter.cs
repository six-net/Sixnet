using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command Before Execute Request Info
    /// </summary>
    public class CommandBeforeExecuteParameter
    {
        #region Propertys

        /// <summary>
        /// ICommand object
        /// </summary>
        public ICommand Command
        {
            get; set;
        }

        /// <summary>
        /// Command Behavior
        /// </summary>
        public CommandBehavior CmdBehavior
        {
            get; set;
        }

        /// <summary>
        /// Data
        /// </summary>
        public object Data
        {
            get; set;
        }

        #endregion
    }
}
