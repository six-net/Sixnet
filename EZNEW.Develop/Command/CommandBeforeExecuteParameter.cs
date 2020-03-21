using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command before execute request info
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
        /// command behavior
        /// </summary>
        public CommandBehavior CmdBehavior
        {
            get; set;
        }

        /// <summary>
        /// data
        /// </summary>
        public object Data
        {
            get; set;
        }

        #endregion
    }
}
