using System;
using System.Collections.Generic;
using EZNEW.Internal.MessageQueue;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command executed internal message command
    /// </summary>
    [Serializable]
    internal class CommandCallbackEventInternalMessageCommand : IInternalMessageQueueCommand
    {

        IEnumerable<Tuple<CommandCallbackEventHandler, CommandCallbackEventParameter>> eventHandlers = null;

        internal CommandCallbackEventInternalMessageCommand(IEnumerable<Tuple<CommandCallbackEventHandler, CommandCallbackEventParameter>> eventHandlers)
        {
            this.eventHandlers = eventHandlers;
        }

        /// <summary>
        /// Execute command
        /// </summary>
        public void Run()
        {
            if (eventHandlers.IsNullOrEmpty())
            {
                return;
            }
            foreach (var operation in eventHandlers)
            {
                operation.Item1(operation.Item2);
            }
        }
    }
}
