using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EZNEW.Internal.MessageQueue
{
    /// <summary>
    /// Internal message queue command
    /// </summary>
    public interface IInternalMessageQueueCommand
    {
        #region Run internal message queue command

        /// <summary>
        /// Run internal message queue command
        /// </summary>
        void Run();

        #endregion
    }
}
