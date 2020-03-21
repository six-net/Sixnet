using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// command callback parameter
    /// </summary>
    public class CommandCallbackParameter
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
        public object Data { get; set; }

        /// <summary>
        /// command execute success
        /// </summary>
        public bool ExecuteSuccess
        {
            get;set;
        }

        #endregion
    }
}
