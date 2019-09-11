using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command Callback Parameter
    /// </summary>
    public class CommandCallbackParameter
    {
        #region Propertys

        /// <summary>
        /// ICommand Object
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
