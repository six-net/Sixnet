// "Company © 2025. All rights reserved."

using Sixnet.Development.Data.Client;
using Sixnet.Development.Data.Command;

namespace Sixnet.Development.Events.Data
{
    /// <summary>
    /// Checked data event
    /// </summary>
    public class SixnetCheckedDataEvent : SixnetBaseDataEvent
    {
        public SixnetCheckedDataEvent()
        {
            EventType = SixnetDataEventType.Checked;
        }

        /// <summary>
        /// Whether has value
        /// </summary>
        public bool HasValue { get; set; }

        public static SixnetCheckedDataEvent Create(ISixnetDataClient dataClient, SixnetDataCommand command, bool hasValue)
        {
            var dataEvent = Create<SixnetCheckedDataEvent>(dataClient, command);
            dataEvent.HasValue = hasValue;
            return dataEvent;
        }
    }
}
