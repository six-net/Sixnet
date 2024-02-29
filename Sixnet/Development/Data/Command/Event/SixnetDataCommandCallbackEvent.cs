using System;

namespace Sixnet.Development.Data.Command.Event
{
    /// <summary>
    /// Command executed event parameter
    /// </summary>
    [Serializable]
    public class SixnetDataCommandCallbackEvent
    {
        /// <summary>
        /// Gets or sets the command
        /// </summary>
        public SixnetDataCommand Command { get; set; }
    }
}
