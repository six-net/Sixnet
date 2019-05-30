using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command Callback Request
    /// </summary>
    public class CommandCallbackRequest
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
        /// Callback Data
        /// </summary>
        public object CallbackData { get; set; }

        #endregion
    }
}
