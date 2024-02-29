namespace Sixnet.Development.Event
{
    /// <summary>
    /// Sixnet time  event handler options
    /// </summary>
    public class TimeEventHandlerOptions
    {
        /// <summary>
        /// Whether execution async
        /// </summary>
        public bool Async { get; set; }

        /// <summary>
        /// Gets or sets the event trigger time
        /// </summary>
        public EventTriggerTime TriggerTime { get; set; } = EventTriggerTime.Immediately;
    }
}
