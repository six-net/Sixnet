using System;

namespace EZNEW.Develop.Command
{
    /// <summary>
    /// Command executing event parameter
    /// </summary>
    [Serializable]
    public class CommandStartingEventParameter
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public ICommand Command
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the command behavior
        /// </summary>
        public CommandBehavior CommandBehavior
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the parameter data
        /// </summary>
        public object Data
        {
            get; set;
        }
    }
}
