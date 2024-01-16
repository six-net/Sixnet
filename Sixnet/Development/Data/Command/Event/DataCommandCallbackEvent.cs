using System;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Command executed event parameter
    /// </summary>
    [Serializable]
    public class DataCommandCallbackEvent
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public DataCommand Command { get; set; }
    }
}
