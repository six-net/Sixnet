using System;
using System.Collections.Generic;
using EZNEW.Internal.MessageQueue;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command executing internal message command
    /// </summary>
    [Serializable]
    internal class CommandStartingEventInternalMessageCommand : IInternalMessageQueueCommand
    {
        IEnumerable<Tuple<CommandStartingEventHandler, CommandStartingEventParameter>> eventHandlers = null;

        internal CommandStartingEventInternalMessageCommand(IEnumerable<Tuple<CommandStartingEventHandler, CommandStartingEventParameter>> eventHandlers)
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
