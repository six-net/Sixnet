using System;

namespace EZNEW.Development.Command
{
    /// <summary>
    /// Command starting event result
    /// </summary>
    [Serializable]
    public class CommandStartingEventExecutionResult
    {
        /// <summary>
        /// Indecates whether break this command
        /// </summary>
        public bool BreakCommand { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets the default success result
        /// </summary>
        public readonly static CommandStartingEventExecutionResult DefaultSuccess = new CommandStartingEventExecutionResult();
    }
}
