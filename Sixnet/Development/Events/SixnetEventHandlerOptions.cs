// "Company © 2025. All rights reserved."

namespace Sixnet.Development.Events
{
    /// <summary>
    /// Sixnet event handler options
    /// </summary>
    public class SixnetEventHandlerOptions
    {
        /// <summary>
        /// Whether execution async
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Gets or sets the event trigger time
        /// </summary>
        public SixnetEventTriggerTime TriggerTime { get; set; } = SixnetEventTriggerTime.Immediately;
    }
}
