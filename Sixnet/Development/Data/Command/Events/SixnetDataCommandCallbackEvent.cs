// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Data.Command.Events
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
